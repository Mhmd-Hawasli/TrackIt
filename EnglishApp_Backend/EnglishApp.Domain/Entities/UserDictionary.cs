using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class UserDictionary
    {
        [Key]
        public int DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relations
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public List<Word> DictionaryWords { get; set; }
        public List<WordConfidence> DictionaryWordConfidences { get; set; }

    }
}
