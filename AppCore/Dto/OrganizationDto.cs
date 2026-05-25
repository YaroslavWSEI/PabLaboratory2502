using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Dto;

public record OrganizationDto : ContactBaseDto
{
    public string Name { get; init; } = string.Empty;
    public string? RegistrationNumber { get; init; }
    public OrganizationType Type { get; init; }
    public int MemberCount { get; init; }

    public static OrganizationDto FromEntity(Organization o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Email = o.Email,
        Phone = o.Phone,
        RegistrationNumber = o.RegistrationNumber,
        Type = o.Type,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        MemberCount = o.Members?.Count ?? 0,
        Address = o.Address is null ? null : new AddressDto(
            o.Address.Street,
            o.Address.City,
            o.Address.ZipCode,
            o.Address.Country.ToString(),
            AddressType.Main
        )
    };
}

public record OrganizationDetailDto : ContactBaseDto
{
    public string Name { get; init; } = string.Empty;
    public string? RegistrationNumber { get; init; }
    public OrganizationType Type { get; init; }
    public List<PersonDto> Members { get; init; } = new();

    public static OrganizationDetailDto FromEntity(Organization o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Email = o.Email,
        Phone = o.Phone,
        RegistrationNumber = o.RegistrationNumber,
        Type = o.Type,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        Members = o.Members?.Select(PersonDto.FromEntity).ToList() ?? new(),
        Address = o.Address is null ? null : new AddressDto(
            o.Address.Street,
            o.Address.City,
            o.Address.ZipCode,
            o.Address.Country.ToString(),
            AddressType.Main
        )
    };
}

public record CreateOrganizationDto(
    string Name,
    string Email,
    string Phone,
    string? RegistrationNumber,
    OrganizationType Type,
    AddressDto? Address
);

public record UpdateOrganizationDto(
    Guid Id,
    string? Name,
    string? Email,
    string? Phone,
    string? RegistrationNumber,
    OrganizationType? Type,
    AddressDto? Address,
    ContactStatus? Status
);