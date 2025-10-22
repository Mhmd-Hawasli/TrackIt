using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.DictionaryDto;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.Services
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
