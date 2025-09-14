using AutoMapper;
using TrackItApp.Domain.Entities;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.User;

namespace TrackItApp.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                .ForMember(dest => dest.BackupEmail, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserTypeID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(value => DateTime.UtcNow))
                .ForMember(dest => dest.IsTwoFactorEnabled, opt => opt.MapFrom(value => false))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(value => false))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(value => false));

            CreateMap<User, RegisterResponse>();
           

           
            CreateMap<User, LoginResponse>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

            CreateMap<User, GetUserResponse>();
        }
    }
}
