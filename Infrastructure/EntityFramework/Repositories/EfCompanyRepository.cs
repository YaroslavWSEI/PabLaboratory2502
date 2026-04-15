using AppCore.Interfaces;
using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfCompanyRepository(ContactsDbContext context) 
    : EfGenericRepository<Company>(context.Companies), ICompanyRepository
{
    public async Task<Company?> GetByNipAsync(string nip)
    {
        // Assuming you add a NIP property to Company model later
        return await context.Companies.FirstOrDefaultAsync(c => c.Email == nip); 
    }

    public async Task<IEnumerable<Company>> SearchByNameAsync(string name)
    {
        return await context.Companies
            .Where(c => c.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        return await context.People
            .Where(p => p.EmployerId == companyId)
            .ToListAsync();
    }
}