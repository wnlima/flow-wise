using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlowWise.Services.Lancamentos.Infrastructure.Persistence;

public class LancamentosDbContextFactory : IDesignTimeDbContextFactory<LancamentosDbContext>
{
    public LancamentosDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                        .Build();

        var builder = new DbContextOptionsBuilder<LancamentosDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Server=localhost;Database=flowwise_lancamentos_db;User Id=flowwise_user;Password=flowwise_password;;TrustServerCertificate=True";
            Console.WriteLine($"[LancamentosDbContextFactory] Usando connection string padrão para design-time: {connectionString}");
        }

        builder.UseNpgsql(
                       connectionString,
                       b => b.MigrationsAssembly("FlowWise.Services.Lancamentos.Infrastructure")
                );

        return new LancamentosDbContext(builder.Options);
    }
}