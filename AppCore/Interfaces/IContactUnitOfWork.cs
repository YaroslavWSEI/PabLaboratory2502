using AppCore.Repositories;

namespace AppCore.Interfaces;

public interface IContactUnitOfWork : IAsyncDisposable
{
    IPersonRepository Persons { get; }
    IOrganizationRepository Organizations { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}