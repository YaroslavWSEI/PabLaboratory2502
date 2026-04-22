using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Enums;
using Infrastructure.Context;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Linq;
using CoreApp.Dtos.Auth;
using Infrastructure.Entities;

namespace Infrastructure.Security;

public class AuthService : IAuthService
{
    private readonly UserManager<CrmUser> _userManager;
    private readonly ContactsDbContext _context;
    private readonly JwtSettings _jwtOptions;

    public AuthService(
        UserManager<CrmUser> userManager,
        ContactsDbContext context,
        JwtSettings jwtOptions)
    {
        _userManager = userManager;
        _context     = context;
        _jwtOptions  = jwtOptions;
    }

    // ───────── LOGIN ─────────
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
                   ?? throw new Exception("Nieprawidłowy email lub hasło.");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new Exception("Nieprawidłowy email lub hasło.");
        }

        if (user.Status != SystemUserStatus.Active)
            throw new Exception("Konto jest nieaktywne.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new Exception("Konto jest zablokowane.");

        await _userManager.ResetAccessFailedCountAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    Task<AuthResponseDto> IAuthService.RefreshTokenAsync(RefreshTokenDto dto)
    {
        return RefreshTokenAsync(dto);
    }

    // ───────── REFRESH ─────────
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Nieprawidłowy token.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new Exception("Użytkownik nie istnieje.");

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t =>
                t.Token == dto.RefreshToken &&
                t.UserId == userId)
            ?? throw new Exception("Nieprawidłowy refresh token.");

        if (!refreshToken.IsActive)
            throw new Exception("Refresh token wygasł lub został odwołany.");

        var newResponse = await GenerateAuthResponseAsync(user);

        refreshToken.Revoke(newResponse.RefreshToken);
        await _context.SaveChangesAsync();

        return newResponse;
    }

    // ───────── REVOKE ─────────

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken)
            ?? throw new Exception("Token nie istnieje.");

        if (!token.IsActive)
            throw new Exception("Token jest już nieaktywny.");

        token.Revoke();
        await _context.SaveChangesAsync();
    }

    // ───────── JWT + RESPONSE ─────────
    private async Task<AuthResponseDto> GenerateAuthResponseAsync(CrmUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new AuthResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Department = user.Department,
                Status = user.Status,
                Roles = roles.ToList()
            }
        };
    }

    // ───────── ACCESS TOKEN ─────────
    private string GenerateAccessToken(CrmUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("department", user.Department),
            new("status", user.Status.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var credentials = new SigningCredentials(
            _jwtOptions.GetSymmetricKey(),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ───────── REFRESH TOKEN ─────────
    private async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
            token.Revoke();

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    // ───────── VALIDATE EXPIRED TOKEN ─────────
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = _jwtOptions.GetSymmetricKey()
        };

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(
            accessToken,
            parameters,
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Nieprawidłowy token.");
        }

        return principal;
    }
}