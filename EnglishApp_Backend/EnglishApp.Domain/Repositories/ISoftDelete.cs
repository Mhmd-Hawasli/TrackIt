using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Repositories
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
