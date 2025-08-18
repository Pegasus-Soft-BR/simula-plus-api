using AutoMapper;
using Domain.Common;
using Domain.DTOs.Exam;

namespace Domain.AutoMapper;

public class DtoToDomainMappingProfile : Profile
{
    public DtoToDomainMappingProfile() : this("Profile")
    {
    }

    protected DtoToDomainMappingProfile(string profileName) : base(profileName)
    {
        CreateMap<ExamDto, Exam>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Result<ExamDto>, Result<Exam>>();

        CreateMap<CreateExamDto, Exam>();
        CreateMap<CreateExamQuestionDto, Question>();

        CreateMap<QuestionDto, Question>();
    }
}