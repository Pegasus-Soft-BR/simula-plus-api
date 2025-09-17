using Microsoft.Extensions.Configuration;

namespace Infra.Database.Providers;

public static class ExamSearchProviderFactory
{
    public static IExamSearchProvider Create(IConfiguration configuration)
    {
        var dbProvider = configuration["DatabaseProvider"] ?? "SqlServer";
        dbProvider = dbProvider.ToLower();

        return dbProvider switch
        {
            "postgres" => new PostgresExamSearchProvider(),
            "sqlite" => new SqliteExamSearchProvider(),
            _ => new SqlServerExamSearchProvider()
        };
    }
}