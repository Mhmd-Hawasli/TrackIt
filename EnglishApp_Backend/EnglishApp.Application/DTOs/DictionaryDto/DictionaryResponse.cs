using EnglishApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.DTOs.DictionaryDto
{
    public class DictionaryResponse
    {
        public int DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WordConfidenceResponse> Confidences { get; set; }

    }
    public class WordConfidenceResponse
    {
        public int ConfidenceId { get; set; }
        public int ConfidenceNumber { get; set; }
        public int ConfidencePeriod { get; set; }
    }
}
