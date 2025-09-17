using Domain;
using Microsoft.EntityFrameworkCore;
using MockExams.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Database.Providers;

public class PostgresExamSearchProvider : IExamSearchProvider
{
    public async Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<Exam>();

        // 1. Normaliza o input
        var termNormalized = term.ToNormalizedSearchText();

        // 2. Quebra em tokens e monta query no padrão "token:* & token:*"
        var tokens = termNormalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => $"{t}:*");

        var tsQuery = string.Join(" & ", tokens);

        // 3. Usa a coluna SearchText
        return await ctx.Exams
            .FromSqlInterpolated($@"
            SELECT * FROM ""Exams""
            WHERE to_tsvector('portuguese', ""SearchText"")
                  @@ to_tsquery('portuguese', {tsQuery})
            ORDER BY ""CreatedAt"" DESC
            LIMIT 10")
            .ToListAsync();
    }

}