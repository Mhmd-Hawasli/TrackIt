using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.DictionaryDto;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;

namespace TrackItApp.Application.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public DictionaryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Task<ApiResponse<DictionaryResponse>> AddDictionaryAsync(AddDictionaryRequest request, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
