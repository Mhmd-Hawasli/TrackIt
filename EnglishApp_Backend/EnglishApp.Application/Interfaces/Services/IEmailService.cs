using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<string> SendEmailVerificationCode(int userId, string email, string deviceId, CodeType codeType);
    }
}
