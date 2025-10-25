using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        //True Response
        public ApiResponse(T? data = default, string? message = null)
        {
            Succeeded = true;
            Data = data;
            Message = message ?? "Operation Successful";
            Errors = null;
        }

        //False Response
        public ApiResponse(string? message, List<string>? errors = null)
        {
            Succeeded = false;
            Data = default(T);
            Message = message ?? "An error occurred.";
            Errors = errors;
        }

        //Default Response
        public ApiResponse(bool succeeded, T? data, string? message, List<string>? errors = null)
        {
            var alternativeMessage = succeeded ? "Operation Successful" : "An error occurred.";
            Succeeded = succeeded;
            Data = data ?? default;
            Message = message ?? alternativeMessage;
            Errors = errors ?? null;
        }

        //ModelState error response 
        public ApiResponse(ModelStateDictionary modelState)
        {
            Succeeded = false;
            Data = default;
            Message = "Validation Failed";
            Errors = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
}
