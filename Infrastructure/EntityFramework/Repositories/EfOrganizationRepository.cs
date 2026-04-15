using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfOrganizationRepository(ContactsDbContext context) 
    : EfGenericRepository<Organization>(context.Organizations), IOrganizationRepository
{
    public async Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        return await context.People
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
    public async Task<Organization?> GetByNameAsync(string name)
    {
        return await context.Organizations.FirstOrDefaultAsync(o => o.Name == name);
    }
}