using AppCore.Dto;

namespace AppCore.Interfaces;

public interface IOrganizationService
{
    Task<PagedResult<OrganizationDto>> GetAllPaged(int page, int size);
    Task<OrganizationDetailDto?> GetById(Guid id);
    Task<OrganizationDto> AddOrganization(CreateOrganizationDto dto);
    Task<OrganizationDto?> UpdateOrganization(UpdateOrganizationDto dto);
    Task DeleteOrganization(Guid id);

    Task<List<PersonDto>> GetMembers(Guid organizationId);
    Task<bool> AddMember(Guid organizationId, Guid personId);
    Task<bool> RemoveMember(Guid organizationId, Guid personId);

    Task<List<OrganizationDto>> Search(string? name, string? emailDomain);
}