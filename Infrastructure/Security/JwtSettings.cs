using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Security;

public class JwtSettings
{
    private const string Section = "Jwt";
    private readonly IConfiguration _configuration;

    public JwtSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Issuer =>
        _configuration[$"{Section}:Issuer"]
        ?? throw new InvalidOperationException("Issuer is not set.");

    public string Audience =>
        _configuration[$"{Section}:Audience"]
        ?? throw new InvalidOperationException("Audience is not set.");

    public string Secret =>
        _configuration[$"{Section}:SecretKey"]
        ?? throw new InvalidOperationException("Secret key is not set.");

    public int ExpirationInMinutes =>
        _configuration.GetValue<int>($"{Section}:ExpiryInMinutes");

    public int RefreshTokenDays =>
        _configuration.GetValue<int>($"{Section}:RefreshTokenDays");

    public SymmetricSecurityKey GetSymmetricKey() =>
        new(Encoding.UTF8.GetBytes(Secret));
}