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

        //Relations
        public int DictionaryID { get; set; }
        public int? ConfidenceID { get; set; }
    }
}
