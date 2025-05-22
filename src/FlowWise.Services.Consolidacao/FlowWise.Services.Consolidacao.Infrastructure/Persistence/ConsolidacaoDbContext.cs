using FlowWise.Services.Consolidacao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlowWise.Services.Consolidacao.Infrastructure.Persistence;

public class ConsolidacaoDbContext : DbContext
{
    public DbSet<SaldoDiario> SaldosDiarios { get; set; }

    public ConsolidacaoDbContext(DbContextOptions<ConsolidacaoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public void EnsureDatabaseCreated()
    {
        Database.EnsureCreated();
    }
}