using AppCore.Models;

namespace AppCore.Repositories;

public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
{
    Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId);
    Task<Organization?> GetByNameAsync(string name);
}