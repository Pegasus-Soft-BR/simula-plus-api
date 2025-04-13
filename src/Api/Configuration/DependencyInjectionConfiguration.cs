using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Domain.Validators;
using MockExams.Infra.CrossCutting.Identity;
using MockExams.Infra.CrossCutting.Identity.Interfaces;
using MockExams.Infra.Database.UoW;
using MockExams.Service;
using MockExams.Jobs;
using Domain.DTOs;
using MockExams.Infra.Email;
using MockExams.Infra.AwsS3;
using MockExams.Infra.Sms;
using MockExams.Infra.UrlShortener;
using MockExams.Lgpd;
using Domain;
using Domain.DTOs;
using Infra.IA;
using Service.Exam.Generator;

namespace MockExams.Api.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection DependencyInjection(
       this IServiceCollection services)
    {
        //services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserEmailService, UserEmailService>();
        services.AddScoped<IContactUsService, ContactUsService>();
        services.AddScoped<IContactUsEmailService, ContactUsEmailService>();
        services.AddScoped<ILgpdService, LgpdService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IExamGeneratorService, ExamGeneratorService>();

        //validators
        services.AddScoped<IValidator<User>, UserValidator>();
        services.AddScoped<IValidator<ContactUs>, ContactUsValidator>();
        services.AddScoped<IValidator<Exam>, ExamValidator>();

        //Auth
        services.AddScoped<IApplicationSignInManager, ApplicationSignInManager>();

        //Infra Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IEmailTemplate, EmailTemplate>();
        services.AddSingleton<IAwsS3Service, AwsS3Service>();
        services.AddSingleton<ISmsService, SmsServiceTwilio>();
        services.AddSingleton<IUrlShortener, UrlShortener>();
        services.AddSingleton<IIAClient, ChatGptClient>();

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //Jobs
        services.AddScoped<IJobExecutor, JobExecutor>();
        services.AddScoped<ExampleJob>();

        return services;
    }
}