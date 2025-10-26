using AutoMapper;
using EnglishApp.Application.DTOs.DictionaryDto;
using EnglishApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.Mapping
{
    public class DictionaryProfile : Profile
    {
        public DictionaryProfile()
        {
            CreateMap<UserDictionary, DictionaryResponse>()
                .ForMember(dest => dest.Confidences, opt => opt.MapFrom(src => src.DictionaryWordConfidences));
        }
    }
}
