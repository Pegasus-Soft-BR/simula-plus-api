using MockExams.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MockExams.Api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration config)
    {
        var dbProvider = config["DatabaseProvider"];

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseLazyLoadingProxies();

            switch (dbProvider)
            {
                case "Postgres":
                    options.UseNpgsql(config.GetConnectionString("PostgresConnection"));
                    break;

                case "Sqlite":
                case "SQLite":
                    options.UseSqlite(config.GetConnectionString("SqliteConnection"));
                    break;

                default:
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                    break;
            }
        });

        return services;
    }
}