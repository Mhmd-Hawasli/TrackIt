using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.DTOs.User;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //From DTO to Model
            CreateMap<CreateUserRequest, User>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.BackupEmail, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserTypeID, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Equals(true))
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
