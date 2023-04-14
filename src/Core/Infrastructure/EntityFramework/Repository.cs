using Microsoft.EntityFrameworkCore;
using Core.Domain;

namespace Core.Infrastructure;

public class Repository<T, TDbContext> : IRepository<T>
    where T : class
    where TDbContext : DbContext, ISequence
{
    protected TDbContext _context;

    public Repository(TDbContext context)
    {
        _context = context;
    }

    public Task<int> GetNextValue()
    {
        return _context.GetNextValue<T>();
    }

    public Task<long> GetNextLongValue()
    {
        return _context.GetNextLongValue<T>();
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Add(T[] entity)
    {
        _context.Set<T>().AddRange(entity);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<T> Get(params object[] keyValues)
    {
        var entity = await _context.Set<T>().FindAsync(keyValues);

        if (entity == null)
        {
            throw new NotFoundException<T>(keyValues);
        }

        return entity;
    }
}
