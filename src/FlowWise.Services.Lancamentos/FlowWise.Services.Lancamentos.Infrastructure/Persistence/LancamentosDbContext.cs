using FlowWise.Services.Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlowWise.Services.Lancamentos.Infrastructure.Persistence;

public class LancamentosDbContext : DbContext
{
    public DbSet<Lancamento> Lancamentos { get; set; }

    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Lancamento>().HasQueryFilter(l => !l.IsDeleted);
        base.OnModelCreating(modelBuilder);
    }

    public void EnsureDatabaseCreated()
    {
        Database.EnsureCreated();
    }
}