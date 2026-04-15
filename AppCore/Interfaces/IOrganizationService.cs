using AppCore.Dto; // Assuming you have or will create Organization DTOs
using AppCore.Models;

namespace AppCore.Interfaces;

public interface IOrganizationService
{
    Task<object?> AddOrganization(object organizationDto); 
    Task<object?> GetById(Guid id);
    Task<IEnumerable<object>> GetAll();
    Task<bool> AddMemberToOrganization(Guid organizationId, Guid personId);
}