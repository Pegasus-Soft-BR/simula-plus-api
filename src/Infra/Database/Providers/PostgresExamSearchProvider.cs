using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Database.Providers;

public class PostgresExamSearchProvider : IExamSearchProvider
{
    public async Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string termNormalized)
    {
        return await ctx.Exams
            .FromSqlInterpolated($@"
                SELECT * FROM ""Exams""
                WHERE to_tsvector('portuguese', ""Title"" || ' ' || ""Description"")
                      @@ plainto_tsquery('portuguese', {termNormalized})
                ORDER BY ""CreatedAt"" DESC
                LIMIT 10")
            .ToListAsync();
    }
}