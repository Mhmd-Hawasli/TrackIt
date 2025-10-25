using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.DTOs.DictionaryDto
{
    public class DictionaryRequest
    {
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
        public List<int>? ConfidencePeriods { get; set; } = new();
    }
   
}
