using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class UserType
    {
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }

        //Relations
        public ICollection<User> Users { get; set; }
    }
}
