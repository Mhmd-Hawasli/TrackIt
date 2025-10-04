using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.DictionaryDto
{
    public class DictionaryResponse
    {
        public int DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDescription { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
