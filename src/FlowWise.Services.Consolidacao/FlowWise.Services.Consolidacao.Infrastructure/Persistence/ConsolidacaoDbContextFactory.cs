using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlowWise.Services.Consolidacao.Infrastructure.Persistence;

public class ConsolidacaoDbContextFactory : IDesignTimeDbContextFactory<ConsolidacaoDbContext>
{
    public ConsolidacaoDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                        .Build();

        var builder = new DbContextOptionsBuilder<ConsolidacaoDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Server=localhost;Database=flowwise_consolidacao_db;User Id=flowwise_user;Password=flowwise_password;;TrustServerCertificate=True";
            Console.WriteLine($"[ConsolidacaoDbContextFactory] Usando connection string padrão para design-time: {connectionString}");
        }

        builder.UseNpgsql(
                       connectionString,
                       b => b.MigrationsAssembly("FlowWise.Services.Consolidacao.Infrastructure")
                );

        return new ConsolidacaoDbContext(builder.Options);
    }
}