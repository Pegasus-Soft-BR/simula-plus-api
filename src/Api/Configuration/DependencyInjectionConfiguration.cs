using Domain;
using Domain.Validators;
using FluentValidation;
using Infra.Database.UoW;
using Infra.IA;
using Infra.PegasusApi;
using Microsoft.Extensions.DependencyInjection;
using MockExams.Service;
using Service.Exam.Generator;

namespace MockExams.Api.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection DependencyInjection(
       this IServiceCollection services)
    {
        //services
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IExamGeneratorService, ExamGeneratorService>();
        services.AddScoped<IQuestionService, QuestionService>();

        //validators
        services.AddScoped<IValidator<Exam>, ExamValidator>();
        services.AddScoped<IValidator<Question>, QuestionValidator>();

        //Infra Services
        services.AddSingleton<IIAClient, ChatGptClient>();
        services.AddSingleton<IPegasusApiClient, PegasusApiClient>();

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}