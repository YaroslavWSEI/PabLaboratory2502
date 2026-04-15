using AppCore.Interfaces;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork : IContactUnitOfWork
{
    public IPersonRepository Persons { get; }
    public ICompanyRepository Companies { get; }
    public IOrganizationRepository Organizations { get; }

    public MemoryContactUnitOfWork(
        IPersonRepository personRepository, 
        ICompanyRepository companyRepository, 
        IOrganizationRepository organizationRepository)
    {
        Persons = personRepository;
        Companies = companyRepository;
        Organizations = organizationRepository;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}