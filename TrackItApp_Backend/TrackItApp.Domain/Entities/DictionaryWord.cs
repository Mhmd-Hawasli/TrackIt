using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class DictionaryWord
    {
        public int WordID { get; set; }
        public string WordText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime lastUpdatedAt { get; set; }
        public DateTime NextReview { get; set; }
        public string? Pronunciation { get; set; }
        public string? Sources { get; set; }
        //Relations
        public int DictionaryID { get; set; }
        public Dictionary Dictionary { get; set; }
        public int? ConfidenceID { get; set; }
        public DictionaryWordConfidence DictionaryWordConfidence { get; set; }

        public ICollection<DictionaryReviewHistory> DictionaryReviewHistories { get; set; }
        public ICollection<DictionaryWordDetail> DictionaryWordDetails { get; set; }
    }
}
