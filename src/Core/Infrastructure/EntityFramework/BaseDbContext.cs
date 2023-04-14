using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Core.Application;
using Core.Domain;

namespace Core.Infrastructure;

public class BaseDbContext : DbContext, IUnitOfWork, ISequence, IDomainEventSource
{
    protected readonly DbSchema _dbSchema;
    protected readonly IClock _clock;

    public BaseDbContext(DbContextOptions options, DbSchema dbSchema, IClock clock) : base(options)
    {
        _dbSchema = dbSchema;
        _clock = clock;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrEmpty(_dbSchema.Name))
        {
            modelBuilder.HasDefaultSchema(_dbSchema.Name);
        }
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public IEnumerable<IDomainEvent> Get()
    {
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot>().Select(entry => entry.Entity).ToList();

        var events = aggregateRoots.SelectMany(aggregateRoot => aggregateRoot.Events).ToArray();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearEvents();
        }

        return events;
    }

    public async Task<int> GetNextValue<T>()
    {
        var result = new SqlParameter("@result", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        var query = $"SELECT @result = (NEXT VALUE FOR {typeof(T).Name}_Sequence)";

        if (!string.IsNullOrEmpty(_dbSchema.Name))
        {
            query = $"SELECT @result = (NEXT VALUE FOR [{_dbSchema.Name}].[{typeof(T).Name}_Sequence])";
        }

        await base.Database.ExecuteSqlRawAsync(query, result);

        return (int)result.Value;
    }

    public async Task<long> GetNextLongValue<T>()
    {
        var result = new SqlParameter("@result", SqlDbType.BigInt)
        {
            Direction = ParameterDirection.Output
        };

        var query = $"SELECT @result = (NEXT VALUE FOR {typeof(T).Name}_Sequence)";

        if (!string.IsNullOrEmpty(_dbSchema.Name))
        {
            query = $"SELECT @result = (NEXT VALUE FOR [{_dbSchema.Name}].[{typeof(T).Name}_Sequence])";
        }

        await base.Database.ExecuteSqlRawAsync(query, result);

        return (long)result.Value;
    }

    public IDbContextTransaction? CurrentTransaction { get; private set; }

    public async Task BeginTransaction()
    {
        if (CurrentTransaction != null)
        {
            return;
        }

        CurrentTransaction = await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task Rollback()
    {
        if (CurrentTransaction == null)
        {
            return;
        }

        try
        {
            await CurrentTransaction.RollbackAsync();
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }

    public async Task Commit()
    {
        if (CurrentTransaction == null)
        {
            return;
        }
        try
        {
            await SaveChangesAsync();

            CurrentTransaction.Commit();
        }
        catch
        {
            await Rollback();
            throw;
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }

    public bool IsTransactionOpened()
    {
        return CurrentTransaction != null;
    }
}