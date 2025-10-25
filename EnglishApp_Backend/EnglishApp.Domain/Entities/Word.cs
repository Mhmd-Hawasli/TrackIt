using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class Word
    {
        [Key]
        public int WordId { get; set; }
        public string WordText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime lastUpdatedAt { get; set; }
        public DateTime NextReview { get; set; }
        public string? Pronunciation { get; set; }
        public string? Sources { get; set; }

        //Relations
        
        [ForeignKey("Dictionary")]
        public int DictionaryId { get; set; }
        public UserDictionary Dictionary { get; set; }

        [ForeignKey("DictionaryWordConfidence")]
        public int? ConfidenceId { get; set; }
        public WordConfidence DictionaryWordConfidence { get; set; }

        public List<ReviewHistory> DictionaryReviewHistories { get; set; }
        public List<WordDetail> DictionaryWordDetails { get; set; }
    }
}
