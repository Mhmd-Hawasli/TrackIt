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
using TrackItApp.Domain.Entities;

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
        #region AddDictionaryAsync
        public async Task<ApiResponse<DictionaryResponse>> AddDictionaryAsync(AddDictionaryRequest request, int userId)
        {
            var existDictionary = await _unitOfWork.DictionaryRepository.FirstOrDefaultAsNoTrackingAsync(d => d.CreatedByUserId == userId && d.DictionaryName == request.DictionaryName);
            if (existDictionary != null)
            {
                return new ApiResponse<DictionaryResponse>("Dictionary name already exists. Please choose another name.");
            }

            var dictionary = new Dictionary 
            {

            };



            throw new NotImplementedException();
        }
        #endregion
    }
}
