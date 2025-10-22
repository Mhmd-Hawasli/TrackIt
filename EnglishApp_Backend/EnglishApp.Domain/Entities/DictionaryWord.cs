using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class DictionaryWord
    {
        public int WordId { get; set; }
        public string WordText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime lastUpdatedAt { get; set; }
        public DateTime NextReview { get; set; }
        public string? Pronunciation { get; set; }
        public string? Sources { get; set; }
        //Relations
        public int DictionaryId { get; set; }
        public Dictionary Dictionary { get; set; }
        public int? ConfidenceId { get; set; }
        public DWConfidence DictionaryWordConfidence { get; set; }

        public ICollection<DWReviewHistory> DictionaryReviewHistories { get; set; }
        public ICollection<DWDetail> DictionaryWordDetails { get; set; }
    }
}
