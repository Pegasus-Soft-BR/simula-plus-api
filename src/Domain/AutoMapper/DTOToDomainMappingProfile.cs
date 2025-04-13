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
        CreateMap<UserRegisterDto, User>();
        CreateMap<UserLoginDTO, User>();
        CreateMap<UserDto, User>();
        CreateMap<UserDtoAdmin, User>();

        CreateMap<User, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
            .ForMember(dest => dest.HashCodePassword, opt => opt.Ignore())
            .ForMember(dest => dest.HashCodePasswordExpiryDate, opt => opt.Ignore())
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<ContactUsDTO, ContactUs>();

        CreateMap<CreateExamDto, Exam>();
        CreateMap<CreateExamQuestionDto, Question>();
    }
}