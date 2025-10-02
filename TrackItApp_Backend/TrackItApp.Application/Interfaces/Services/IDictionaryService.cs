using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.DictionaryDto;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IDictionaryService
    {
        Task<ApiResponse<DictionaryResponse>> AddDictionaryAsync(AddDictionaryRequest request, int userId);
    }
}
