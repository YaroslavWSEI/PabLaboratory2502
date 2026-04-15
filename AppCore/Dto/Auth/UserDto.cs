using AppCore.Interfaces;

namespace AppCore.Dto;

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Department { get; init; }
    public SystemUserStatus Status { get; set; }
    public IEnumerable<string> Roles { get; set; }
}