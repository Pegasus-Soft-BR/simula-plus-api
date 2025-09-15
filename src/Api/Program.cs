﻿using Domain.AutoMapper;
using Domain.Common;
using Domain.DTOs;
using Infra.HttpHandlers;
using Infra.IA;
using Infra.PegasusApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using MockExams.Api.Configuration;
using MockExams.Api.Middleware;
using MockExams.Infra.Database;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração de banco de dados. Podemos alternar entre SQL Server, Postgres e SQLite!
builder.Services.AddDatabaseConfiguration(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Serilog
builder.AddSerilogConfiguration();

// HealthChecks
var dbProvider = builder.Configuration["DatabaseProvider"];
var healthChecks = builder.Services.AddHealthChecks();

if (dbProvider == "Postgres")
{
    healthChecks.AddNpgSql(builder.Configuration.GetConnectionString("PostgresConnection"));
}
else if (dbProvider == "SqlServer")
{
    healthChecks.AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
// SQLite não precisa de health check específico

// Configuração de serviços e dependências
builder.Services.AddTransient<LoggingHttpMessageHandler>();
builder.Services.AddTransient<TraceIdPropagationHandler>();
builder.Services.AddHttpClient("DefaultClient")
    .AddHttpMessageHandler<TraceIdPropagationHandler>()
    .AddHttpMessageHandler<LoggingHttpMessageHandler>();

builder.Services.DependencyInjection();
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpContextAccessor();

builder.Services
    .Configure<ServerSettings>(builder.Configuration.GetSection("ServerSettings"))
    .Configure<IASettings>(builder.Configuration.GetSection("IASettings"))
    .Configure<PegasusApiSettings>(builder.Configuration.GetSection("PegasusApiSettings"));

var serverSettings = builder.Configuration
    .GetSection("ServerSettings")
    .Get<ServerSettings>();
AppSettings.ServerSettings = serverSettings;

JWTConfig.RegisterJWT(builder.Services, builder.Configuration);
builder.Services.RegisterSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

// Middlewares
app.UseSerilogRequestLogging();

app.UseExceptionHandlerMiddleware();
app.UseHealthChecks("/hc");

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    }
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simula+ API V1");
});

app.UseRouting();
app.UseCors("AllowAllHeaders");
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

// Seed e migrations
using (var scope = app.Services.CreateScope())
{
    var env = app.Environment;
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    if (env.IsDevelopment() || env.IsStaging())
    {
        var seeder = new DatabaseSeeder(context);
        seeder.Seed();
    }
}

app.Run();
