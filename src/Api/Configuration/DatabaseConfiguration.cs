using MockExams.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MockExams.Api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration config)
    {
        var dbProvider = config["DatabaseProvider"].ToLower();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseLazyLoadingProxies();

            switch (dbProvider)
            {
                case "postgres":
                    options.UseNpgsql(config.GetConnectionString("PostgresConnection"));
                    break;

                case "sqlite":
                    options.UseSqlite(config.GetConnectionString("SqliteConnection"));
                    break;

                default:
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                    break;
            }
        });

        var ctx = services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();

        if (dbProvider == "sqlite") ctx.Database.EnsureCreated();
        else ctx.Database.Migrate();

        var seeder = new DatabaseSeeder(ctx);
        // seeder.Seed();

        return services;
    }
}