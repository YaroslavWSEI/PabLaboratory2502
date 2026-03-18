namespace AppCore.Repositories;
using AppCore.Models;

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> SearchAsync(string query);
    Task<IEnumerable<Person>> GetByEmployerAsync(Guid companyId);
    Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId);
}