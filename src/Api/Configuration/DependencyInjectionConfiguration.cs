using Domain;
using Domain.Validators;
using FluentValidation;
using Infra.IA;
using Microsoft.Extensions.DependencyInjection;
using MockExams.Infra.AwsS3;
using MockExams.Infra.Database.UoW;
using MockExams.Infra.Sms;
using MockExams.Infra.UrlShortener;
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
        services.AddSingleton<IAwsS3Service, AwsS3Service>();
        services.AddSingleton<ISmsService, SmsServiceTwilio>();
        services.AddSingleton<IUrlShortener, UrlShortener>();
        services.AddSingleton<IIAClient, ChatGptClient>();

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}