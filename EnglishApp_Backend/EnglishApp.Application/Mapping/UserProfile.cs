using AutoMapper;
using EnglishApp.Domain.Entities;
using EnglishApp.Application.DTOs.UserDto.Auth;
using EnglishApp.Application.DTOs.UserDto.User;

namespace EnglishApp.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(value => DateTime.UtcNow))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(value => false))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(value => false));

            CreateMap<User, RegisterResponse>();



            CreateMap<User, LoginResponse>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

            CreateMap<User, UserGetByIdResponse>();
            CreateMap<User, UsersResponse>();
            CreateMap<User, UsersWithSoftDeleteResponse>();
        }
    }
}
