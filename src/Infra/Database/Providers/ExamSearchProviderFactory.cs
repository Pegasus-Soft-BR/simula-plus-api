using Microsoft.Extensions.Configuration;

namespace MockExams.Infra.Database.Providers;

public static class ExamSearchProviderFactory
{
    public static IExamSearchProvider Create(IConfiguration configuration)
    {
        var dbProvider = configuration["DatabaseProvider"] ?? "SqlServer";

        return dbProvider switch
        {
            "Postgres" => new PostgresExamSearchProvider(),
            "Sqlite" => new SqliteExamSearchProvider(),
            _ => new SqlServerExamSearchProvider()
        };
    }
}