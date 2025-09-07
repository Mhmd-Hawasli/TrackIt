using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Entities;
using TrackItApp.Application.DTOs.UserDto.Auth;

namespace TrackItApp.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //From DTO to Model
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.BackupEmail, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserTypeID, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<User, RegisterResponse>();
        }
    }
}
