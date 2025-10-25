using AutoMapper;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.DictionaryDto;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Domain.Entities;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ApiResponse<DictionaryResponse>> AddDictionaryAsync(DictionaryRequest request, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Check if a dictionary with the same name already exists for this user
                var existingDictionary = await _unitOfWork.UserDictionaryRepository
                    .FirstOrDefaultAsync(d =>
                        d.CreatedByUserId == userId &&
                        d.DictionaryName == request.DictionaryName);

                if (existingDictionary != null)
                {
                    return new ApiResponse<DictionaryResponse>(
                        "A dictionary with the same name already exists. Please choose another name."
                    );
                }

                // Create a new dictionary entity
                var newDictionary = new UserDictionary
                {
                    DictionaryName = request.DictionaryName.Trim(),
                    DictionaryDescription = request.DictionaryDescription.Trim(),
                    CreatedByUserId = userId
                };

                // Add word confidence periods if provided
                if (request.ConfidencePeriods?.Any() == true)
                {
                    var confidences = request.ConfidencePeriods
                        .Select((period, index) => new WordConfidence
                        {
                            ConfidenceNumber = index + 1,
                            ConfidencePeriod = period
                        })
                        .ToList();

                    newDictionary.DictionaryWordConfidences.AddRange(confidences);
                }

                await _unitOfWork.UserDictionaryRepository.AddAsync(newDictionary);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<DictionaryResponse>(newDictionary);

                return new ApiResponse<DictionaryResponse>(
                    response,
                    "Dictionary created successfully."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                return new ApiResponse<DictionaryResponse>(
                    "An error occurred while creating the dictionary.",
                    new List<string> { ex.Message }
                );
            }
        }
        #endregion



        #region UpdateDictionaryAsync
        public async Task<ApiResponse<DictionaryResponse>> UpdateDictionaryAsync(DictionaryRequest request, int userId)
        {
            var dictionary = await _unitOfWork.UserDictionaryRepository
                .FirstOrDefaultAsync(d => d.CreatedByUserId == userId && d.DictionaryName == request.DictionaryName);

            if (dictionary == null)
                return new ApiResponse<DictionaryResponse>("Dictionary not found for this user.");

            // Prevent renaming to an existing name
            if (!string.IsNullOrEmpty(request.DictionaryName) &&
                await _unitOfWork.UserDictionaryRepository.FirstOrDefaultAsync(d =>
                    d.CreatedByUserId == userId &&
                    d.DictionaryName == request.DictionaryName &&
                    d.DictionaryId != dictionary.DictionaryId) != null)
            {
                return new ApiResponse<DictionaryResponse>("Another dictionary with this name already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.DictionaryName))
                dictionary.DictionaryName = request.DictionaryName;

            if (!string.IsNullOrWhiteSpace(request.DictionaryDescription))
                dictionary.DictionaryDescription = request.DictionaryDescription;

            _unitOfWork.UserDictionaryRepository.Update(dictionary);
            await _unitOfWork.CompleteAsync();

            var response = _mapper.Map<DictionaryResponse>(dictionary);
            return new ApiResponse<DictionaryResponse>(true, response, "Dictionary has been updated successfully");
        }
        #endregion

        #region DeleteDictionaryAsync
        public async Task<ApiResponse<object>> DeleteDictionaryAsync(int dictionaryId, int userId)
        {
            var dictionary = await _unitOfWork.UserDictionaryRepository
                .FirstOrDefaultAsync(d => d.CreatedByUserId == userId && d.DictionaryId == dictionaryId);

            if (dictionary == null)
                return new ApiResponse<object>("Dictionary not found for this user.");

            _unitOfWork.UserDictionaryRepository.Remove(dictionary);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "Dictionary has been deleted successfully");
        }
        #endregion

        #region GetDictionaryByIdAsync
        public async Task<ApiResponse<DictionaryResponse>> GetDictionaryByIdAsync(int dictionaryId)
        {
            var dictionary = await _unitOfWork.UserDictionaryRepository.FirstOrDefaultAsync(d => d.DictionaryId == dictionaryId);
            if (dictionary == null)
                return new ApiResponse<DictionaryResponse>("Dictionary not found in database.");

            var response = _mapper.Map<DictionaryResponse>(dictionary);
            return new ApiResponse<DictionaryResponse>(response);
        }
        #endregion

        #region GetAllDictionariesByUserAsync
        public async Task<ApiResponse<ICollection<DictionaryResponse>>> GetAllDictionariesByUserAsync(int userId)
        {
            var dictionaries = await _unitOfWork.UserDictionaryRepository.FindAsync(d => d.CreatedByUserId == userId);
            var response = _mapper.Map<ICollection<DictionaryResponse>>(dictionaries);

            return new ApiResponse<ICollection<DictionaryResponse>>(response);
        }
        #endregion
    }
}
