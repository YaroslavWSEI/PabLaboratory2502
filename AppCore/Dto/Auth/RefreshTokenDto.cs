namespace CoreApp.Dtos.Auth;

public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);