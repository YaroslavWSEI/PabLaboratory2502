using AppCore.Interfaces;
using AppCore.Repositories;
using Infrastructure.Context;

namespace Infrastructure.UnitOfWork;

public class EfContactsUnitOfWork : IContactUnitOfWork
{
    private readonly ContactsDbContext _context;
    
    public IPersonRepository Persons { get; }
    public ICompanyRepository Companies { get; }
    public IOrganizationRepository Organizations { get; }

    public EfContactsUnitOfWork(
        IPersonRepository personRepository,
        ICompanyRepository companyRepository,
        IOrganizationRepository organizationRepository,
        ContactsDbContext context)
    {
        Persons = personRepository;
        Companies = companyRepository;
        Organizations = organizationRepository;
        _context = context;
    }

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    public Task BeginTransactionAsync() => _context.Database.BeginTransactionAsync();
    public Task CommitTransactionAsync() => _context.Database.CommitTransactionAsync();
    public Task RollbackTransactionAsync() => _context.Database.RollbackTransactionAsync();
}