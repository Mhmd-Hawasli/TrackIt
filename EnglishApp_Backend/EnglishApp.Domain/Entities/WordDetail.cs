using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class WordDetail
    {
        [Key]
        public int WordDetailId { get; set; }
        public string Title { get; set; }
        public string? WordImage { get; set; }
        public string? Example { get; set; }
        public WordType Type { get; set; }
        public string? Arabic { get; set; }
        public string Description { get; set; }

        //Relations
        [ForeignKey("DictionaryWord")]
        public int WordId { get; set; }
        public Word DictionaryWord { get; set; }
    }

    public enum WordType
    {
        None = 0,
        Noun = 1,
        Verb = 2,
        Adjective = 3,
        Adverb = 4,
    };
}
