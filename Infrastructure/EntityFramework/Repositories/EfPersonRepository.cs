using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfPersonRepository(ContactsDbContext context) 
    : EfGenericRepository<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> GetByLastNameAsync(string lastName) 
        => await context.People.Where(p => p.LastName.Contains(lastName)).ToListAsync();

    public async Task<IEnumerable<Person>> GetByEmployerAsync(Guid employerId)
        => await context.People.Where(p => p.EmployerId == employerId).ToListAsync();

    public async Task<IEnumerable<Person>> GetByOrganizationAsync(Guid organizationId)
        => await context.People.Where(p => p.OrganizationId == organizationId).ToListAsync();

    public async Task<IEnumerable<Person>> SearchAsync(string term)
        => await context.People
            .Where(p => p.FirstName.Contains(term) || p.LastName.Contains(term) || p.Email.Contains(term))
            .ToListAsync();
}