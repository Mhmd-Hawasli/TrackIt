using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class ReviewHistory
    {
        [Key]
        public int ReviewId { get; set; }
        public int OldConfidenceNumber { get; set; }
        public int NewConfidenceNumber { get; set; }
        public DateTime ReviewedAt { get; set; }

        //Relations
        [ForeignKey("DictionaryWord")]
        public int WordId { get; set; }
        public Word DictionaryWord { get; set; }
    }
}
