using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class DictionaryWordDetail
    {
        public int WordDetailID { get; set; }
        public string Title { get; set; }
        public string? WordImage { get; set; }
        public string? Example { get; set; }
        public WordType Type { get; set; }
        public string? Arabic { get; set; }
        public string Description { get; set; }

        //Relations
    }

    public enum WordType
    {

    };
}
