using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infra.Database;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var dbProvider = configuration["DatabaseProvider"] ?? "SqlServer";
        dbProvider = dbProvider.ToLower();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (dbProvider)
        {
            case "postgres":
                var postgresConnectionString = configuration.GetConnectionString("PostgresConnection");
                optionsBuilder.UseNpgsql(postgresConnectionString);
                break;

            case "sqlite":
                var sqliteConnectionString = configuration.GetConnectionString("SqliteConnection");
                optionsBuilder.UseSqlite(sqliteConnectionString);
                break;

            default:
                var sqlServerConnectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(sqlServerConnectionString);
                break;
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}