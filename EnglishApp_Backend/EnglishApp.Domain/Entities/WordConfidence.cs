using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class WordConfidence
    {
        [Key]
        public int ConfidenceId { get; set; }
        public int ConfidenceNumber { get; set; }
        public int ConfidencePeriod { get; set; }

        //Relations
        [ForeignKey("Dictionary")]
        public int DictionaryId { get; set; }
        public UserDictionary Dictionary { get; set; }
        public List<Word> DictionaryWords { get; set; }
    }
}
