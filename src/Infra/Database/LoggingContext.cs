using JsonDiffPatchDotNet;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helper;

namespace MockExams.Infra.Database;

public static class LoggingContext
{
    private static readonly List<EntityState> entityStates = new List<EntityState>() { EntityState.Added, EntityState.Modified, EntityState.Deleted };

    public static async Task LogChanges(this ApplicationDbContext context)
    {
        var logTime = DateTime.UtcNow;
        const string emptyJson = "{}";
        const string idColumn = "Id";

        Guid? user = null;
        if (!string.IsNullOrEmpty(Thread.CurrentPrincipal?.Identity?.Name))
            user = new Guid(Thread.CurrentPrincipal?.Identity?.Name);

        var changes = context.ChangeTracker.Entries()
            .Where(x => entityStates.Contains(x.State) && x.Entity.GetType().IsSubclassOf(typeof(BaseEntity)))
            .ToList();

        var jdp = new JsonDiffPatch();

        foreach (var item in changes)
        {
            var original = emptyJson;
            var updated = JsonHelper.ToJson(item.CurrentValues.Properties.ToDictionary(pn => pn.Name, pn => item.CurrentValues[pn]));
            var CreatedAt = DateTime.UtcNow;

            if (item.State == EntityState.Modified)
            {
                var dbValues = await item.GetDatabaseValuesAsync();

                if (dbValues != null)
                {
                    original = JsonHelper.ToJson(dbValues.Properties.ToDictionary(pn => pn.Name, pn => dbValues[pn]));
                    CreatedAt = dbValues.GetValue<DateTime>("CreatedAt");
                }
            }

            item.Property("CreatedAt").CurrentValue = CreatedAt;

            string jsonDiff = jdp.Diff(original, updated);

            if (string.IsNullOrWhiteSpace(jsonDiff) == false)
            {
                var EntityDiff = JsonHelper.NormalizeJson(jsonDiff);

                var logEntry = new LogEntry()
                {
                    EntityName = item.Entity.GetType().Name,
                    EntityId = new Guid(item.CurrentValues[idColumn].ToString()),
                    LogDateTime = logTime,
                    Operation = item.State.ToString(),
                    UserId = user,
                    ValuesChanges = EntityDiff,
                };

                context.LogEntries.Add(logEntry);
            }

        }
    }
}
