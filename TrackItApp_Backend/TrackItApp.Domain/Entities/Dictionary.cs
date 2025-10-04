using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class Dictionary
    {
        public int DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relations
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public ICollection<DictionaryWord> DictionaryWords { get; set; }
        public ICollection<DWConfidence> DictionaryWordConfidences { get; set; }

    }
}
