using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfGenericRepository<T>(DbSet<T> set): IGenericRepositoryAsync<T>
    where T: EntityBase
{
    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await set.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await set.ToListAsync();
    }

    public async Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = await set
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var result = new PagedResult<T>(
            items,
            set.Count(),
            page,
            pageSize       
        );
        return result;
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await set.AddAsync(entity);
        return entry.Entity;
    }

    public Task<T> UpdateAsync(T entity)
    {
        var entityEntry = set.Update(entity);
        return Task.FromResult(entityEntry.Entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        var entity = set.Find(id);
        if (entity is null) 
            return Task.CompletedTask;
        set.Remove(entity);
        return Task.CompletedTask;
    }
}