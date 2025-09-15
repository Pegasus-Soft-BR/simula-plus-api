using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockExams.Infra.Database.Providers;

public class PostgresExamSearchProvider : IExamSearchProvider
{
    public async Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string termNormalized)
    {
        var query = @"
                SELECT * FROM ""Exams""
                WHERE to_tsvector('portuguese', ""Title"" || ' ' || ""Description"") @@ plainto_tsquery('portuguese', {0})
                ORDER BY ""CreatedAt"" DESC
                LIMIT 10";

        return await ctx.Exams
            .FromSqlRaw(query, termNormalized)
            .ToListAsync();
    }
}