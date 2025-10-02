using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class DWReviewHistory
    {
        public int ReviewID { get; set; }
        public int OldConfidenceNumber { get; set; }
        public int NewConfidenceNumber { get; set; }
        public DateTime ReviewedAt { get; set; }

        //Relations
        public int WordID { get; set; }
        public DictionaryWord DictionaryWord { get; set; }
    }
}
