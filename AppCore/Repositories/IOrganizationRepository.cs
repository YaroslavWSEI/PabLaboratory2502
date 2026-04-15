using AppCore.Models;

namespace AppCore.Repositories;

public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
{
    // Specific method to get all people belonging to an organization
    Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId);
    
    // You can add search by name or registration number here later
    Task<Organization?> GetByNameAsync(string name);
}