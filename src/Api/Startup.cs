using Api.Configuration;
using Api.Middleware;
using Domain.AutoMapper;
using Domain.DTOs;
using Infra.HttpHandlers;
using Infra.IA;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockExams.Api.Configuration;
using MockExams.Api.Middleware;
using MockExams.Api.Services;
using MockExams.Infra.AwsS3;
using MockExams.Infra.Database;
using MockExams.Infra.Email;
using MockExams.Infra.Sms;
using MockExams.Infra.UrlShortener;
using Rollbar.NetPlatformExtensions;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Text.Json.Serialization;

namespace MockExams.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<LoggingHttpMessageHandler>();

        services.AddHttpClient("DefaultClient")
            .AddHttpMessageHandler<LoggingHttpMessageHandler>(); ;
        
        var isDocker = Environment.GetEnvironmentVariable("IS_DOCKER");
        var connectionStringKey = isDocker == "1" ? "DefaultConnectionDocker" : "DefaultConnection";

        // Serilog
        services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.With<SerilogTraceIdEnricher>()
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            connectionString: Configuration.GetConnectionString(connectionStringKey),
            sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
        );

        // HelthChecks
        RegisterHealthChecks(services, Configuration.GetConnectionString(connectionStringKey));

        services.DependencyInjection();
        services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services
            .AddHttpContextAccessor()
            .Configure<RollbarOptions>(options => Configuration.GetSection("Rollbar").Bind(options))
            .AddRollbarLogger(loggerOptions => loggerOptions.Filter = (loggerName, loglevel) => loglevel >= LogLevel.Trace);

        services.Configure<EmailSettings>(options => Configuration.GetSection("EmailSettings").Bind(options));
        services.Configure<ServerSettings>(options => Configuration.GetSection("ServerSettings").Bind(options));
        services.Configure<AwsS3Settings>(options => Configuration.GetSection("AwsS3Settings").Bind(options));
        services.Configure<SmsSettingsTwillio>(options => Configuration.GetSection("SmsSettingsTwillio").Bind(options));
        services.Configure<UrlShortenerSettings>(options => Configuration.GetSection("UrlShortenerSettings").Bind(options));
        services.Configure<IASettings>(options => Configuration.GetSection("IASettings").Bind(options));

        services.AddHttpContextAccessor();

        JWTConfig.RegisterJWT(services, Configuration);

        services.RegisterSwagger();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllHeaders",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.GetConnectionString(connectionStringKey))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))

             );

        RollbarConfigurator
            .Configure(environment: Configuration.GetSection("Rollbar:Environment").Value,
                       isActive: Configuration.GetSection("Rollbar:IsActive").Value,
                       token: Configuration.GetSection("Rollbar:Token").Value,
                       logLevel: Configuration.GetSection("Rollbar:LogLevel").Value);


        

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<TraceIdOverrideMiddleware>();
        app.UseSerilogRequestLogging();


        bool rollbarActive = Convert.ToBoolean(Configuration.GetSection("Rollbar:IsActive").Value.ToLower());
        if (rollbarActive)
        {
            app.UseRollbarMiddleware();
        }

        app.UseHealthChecks("/hc");
        app.UseExceptionHandlerMiddleware();
        
        app.UseStaticFiles(new StaticFileOptions()
        {
            OnPrepareResponse = (context) =>
            {
                // Enable cors
                context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Book}/{action=Index}/{id?}");

            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });
        });

        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var scopeServiceProvider = serviceScope.ServiceProvider;
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();
            if (env.IsDevelopment() || env.IsStaging())
            {
                var MockExamsSeeder = new DatabaseSeeder(context);
                MockExamsSeeder.Seed();
            }
        }


        
    }

    private void RegisterHealthChecks(IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddSqlServer(connectionString);
    }
}