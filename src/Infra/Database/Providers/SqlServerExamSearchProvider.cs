using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockExams.Infra.Database.Providers;

public class SqlServerExamSearchProvider : IExamSearchProvider
{
    public async Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string termNormalized)
    {
        var keywords = termNormalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(k => k.Length >= 3)
            .Select(k => $"\"{k}*\"");

        var containsClause = string.Join(" AND ", keywords);

        var query = @$"
            SELECT TOP 10 e.*
            FROM CONTAINSTABLE(Exams, (Title, Description), '{containsClause}') AS ft
            JOIN Exams e ON e.Id = ft.[KEY]
            ORDER BY ft.[RANK] DESC";

        return await ctx.Exams
            .FromSqlRaw(query)
            .ToListAsync();
    }
}