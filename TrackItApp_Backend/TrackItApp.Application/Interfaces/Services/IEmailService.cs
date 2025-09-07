using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<string> SendEmailVerificationCode(User userModel, string deviceId, CodeType codeType);
    }
}
