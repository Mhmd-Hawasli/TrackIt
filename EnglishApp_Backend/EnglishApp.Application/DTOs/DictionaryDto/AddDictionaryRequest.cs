using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.DTOs.DictionaryDto
{
    public class AddDictionaryRequest
    {
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
    }
}
