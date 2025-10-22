using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.DictionaryDto;

namespace EnglishApp.Application.Interfaces.Services
{
    public interface IDictionaryService
    {
        Task<ApiResponse<DictionaryResponse>> AddDictionaryAsync(AddDictionaryRequest request, int userId);
    }
}
