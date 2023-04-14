using Core.Domain;
using Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WebAPI.Infrastructure;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(DbContextOptions options, DbSchema dbSchema, IClock clock) : base(options, dbSchema, clock)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
