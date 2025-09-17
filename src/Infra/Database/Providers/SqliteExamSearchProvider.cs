using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Database.Providers;

public class SqliteExamSearchProvider : IExamSearchProvider
{
    public async Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string termNormalized)
    {
        var keywords = termNormalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(k => k.Length >= 2);

        var query = ctx.Exams.AsQueryable();

        foreach (var keyword in keywords)
        {
            query = query.Where(e =>
                e.Title.ToLower().Contains(keyword.ToLower()) ||
                e.Description.ToLower().Contains(keyword.ToLower()));
        }

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Take(10)
            .ToListAsync();
    }
}