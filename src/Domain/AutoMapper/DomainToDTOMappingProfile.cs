using AutoMapper;
using Domain;
using Domain.DTOs;
using Domain.Common;
using System.Collections.Generic;
using Domain.DTOs.Exam;

namespace Domain.AutoMapper;

public class DomainToDTOMappingProfile : Profile
{
    public string BaseUrl { get; set; }

    public DomainToDTOMappingProfile() : this("Profile")
    {
        // todo: obter a url base do appsettings.json
        // IOptions<ServerSettings> settings
        BaseUrl = "https://mockexams.sharebook.com.br";
    }

    protected DomainToDTOMappingProfile(string profileName) : base(profileName)
    {
        CreateMap<User, UserDto>();

        CreateMap<User, UserDtoAdmin>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, UserListDTO>();
        CreateMap<Result<User>, Result<UserDto>>();

        CreateMap<PagedList<User>, PagedList<UserListDTO>>();

        CreateMap<Result<User>, Result<UserDtoAdmin>>();

        CreateMap<AccessHistory, AccessHistoryDto>();

        CreateMap<Exam, ExamDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.GetImageUrl(BaseUrl)));

        CreateMap<PagedList<Exam>, PagedList<ExamDto>>();

        CreateMap<ExamAttempt, ExamAttemptDto>();
        CreateMap<Answer, AnswerDto>();

        CreateMap<Question, QuestionDto>();

        CreateMap<ExamAttempt, MyExamAttemptDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Exam.GetImageUrl(BaseUrl)));

        CreateMap<Answer, QuestionWithAnswerDto>()
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Question.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Question.Title))
            .ForMember(dest => dest.Option1, opt => opt.MapFrom(src => src.Question.Option1))
            .ForMember(dest => dest.Option2, opt => opt.MapFrom(src => src.Question.Option2))
            .ForMember(dest => dest.Option3, opt => opt.MapFrom(src => src.Question.Option3))
            .ForMember(dest => dest.Option4, opt => opt.MapFrom(src => src.Question.Option4))
            .ForMember(dest => dest.Option5, opt => opt.MapFrom(src => src.Question.Option5))
            .ForMember(dest => dest.CorrectOptions, opt => opt.MapFrom(src => src.Question.CorrectOptions));

        CreateMap<ExamAttempt, MyExamAttemptDetailsDto>();

        CreateMap<Exam, MyExamAttemptDetailsDto>()
            .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.GetImageUrl(BaseUrl)));
    }

}