using AppCore.Models;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    public MemoryPersonRepository()
    {
        AddAsync(new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Gender = AppCore.Enums.Gender.Male
        }).Wait();

        AddAsync(new Person
        {
            FirstName = "Anna",
            LastName = "Kowalska",
            Gender = AppCore.Enums.Gender.Female
        }).Wait();
    }

    public Task<IEnumerable<Person>> GetByEmployerAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> SearchAsync(string query)
    {
        throw new NotImplementedException();
    }
}