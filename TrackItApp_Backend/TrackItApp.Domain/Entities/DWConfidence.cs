using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class DWConfidence
    {
        public int ConfidenceId { get; set; }
        public int ConfidenceNumber { get; set; }
        public int ConfidencePeriod { get; set; }

        //Relations
        public int DictionaryId { get; set; }
        public Dictionary Dictionary { get; set; }
        public ICollection<DictionaryWord> DictionaryWords { get; set; }
    }
}
