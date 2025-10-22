using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class DWReviewHistory
    {
        public int ReviewId { get; set; }
        public int OldConfidenceNumber { get; set; }
        public int NewConfidenceNumber { get; set; }
        public DateTime ReviewedAt { get; set; }

        //Relations
        public int WordId { get; set; }
        public DictionaryWord DictionaryWord { get; set; }
    }
}
