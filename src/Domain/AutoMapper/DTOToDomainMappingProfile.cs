using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Exam;

namespace Domain.AutoMapper;

public class DtoToDomainMappingProfile : Profile
{
    public DtoToDomainMappingProfile() : this("Profile")
    {
    }

    protected DtoToDomainMappingProfile(string profileName) : base(profileName)
    {
        CreateMap<CreateExamDto, Exam>();
        CreateMap<CreateExamQuestionDto, Question>();
    }
}