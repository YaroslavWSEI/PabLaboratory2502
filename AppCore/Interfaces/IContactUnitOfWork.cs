using AppCore.Repositories;

namespace AppCore.Interfaces;

public interface IContactUnitOfWork : IAsyncDisposable
{
    IPersonRepository Persons { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}