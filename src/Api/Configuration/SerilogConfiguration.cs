using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Diagnostics;

namespace MockExams.Api.Configuration;

public static class SerilogConfiguration
{
    public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var dbProvider = config["DatabaseProvider"];

        string connectionString = dbProvider == "Postgres"
            ? config.GetConnectionString("PostgresConnection")
            : config.GetConnectionString("DefaultConnection");

        builder.Host.UseSerilog((ctx, lc) =>
        {
            var config = lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .Enrich.With<SerilogTraceIdEnricher>()
                .WriteTo.Console();

            if (dbProvider == "Postgres")
            {
                config.WriteTo.PostgreSQL(connectionString, "Logs", needAutoCreateTable: true);
            }
            else if (dbProvider == "SqlServer")
            {
                config.WriteTo.MSSqlServer(connectionString, new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true
                });
            }
            // SQLITE não está implementado, pois não é recomendado para produção
        });

        return builder;
    }
}


public class SerilogTraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", traceId));
        }
    }
}