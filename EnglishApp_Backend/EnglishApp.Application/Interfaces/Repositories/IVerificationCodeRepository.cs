using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;
using EnglishApp.Domain.Repositories;

namespace EnglishApp.Application.Interfaces.Repositories
{
    public interface IVerificationCodeRepository : IRepositoryBase<VerificationCode>
    {
    }
}
