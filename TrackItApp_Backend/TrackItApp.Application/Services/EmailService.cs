using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Read email settings from appsettings.json
                var emailSettings = _configuration.GetSection("EmailSettings");
                string smtpServer = emailSettings["SmtpServer"]!;
                int port = int.Parse(emailSettings["Port"]!);
                string senderName = emailSettings["SenderName"]!;
                string senderEmail = emailSettings["SenderEmail"]!;
                string password = emailSettings["Password"]!;

                // Create the email message using MimeKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail)); // Set sender
                message.To.Add(new MailboxAddress("", toEmail));               // Set recipient
                message.Subject = subject;                                     // Set email subject
                message.Body = new TextPart("html") { Text = body };           // Set email body in HTML format

                // Connect to SMTP server and send the email using MailKit
                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls); // Connect with TLS
                await client.AuthenticateAsync(senderEmail, password);                                      // Authenticate sender
                await client.SendAsync(message);                                                            // Send email
                await client.DisconnectAsync(true);                                                         // Disconnect from SMTP server

                // Log success
                _logger.LogInformation($"Email sent to {toEmail} with subject: {subject}");
            }
            catch (Exception ex)
            {
                // Log any errors and rethrow
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }

        }


        /// <summary>
        /// Sends a 6-digit verification code to the user's email address.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="email">The email address to send the code to.</param>
        /// <returns>The generated verification code.</returns>
        public async Task<string> SendEmailVerificationCode(int userID, string email, string deviceId, CodeType codeType)
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();
            var verificationEntity = new VerificationCode
            {
                UserID = userID,
                Code = verificationCode,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CodeType = codeType,
                Email = email.ToLower(),
                DeviceID = deviceId
            };

            // Check for an existing, un-expired verification code and remove it before adding a new one
            var existingCode = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.UserID == userID && vc.DeviceID == deviceId);
            if (existingCode != null)
            {
                existingCode.Code = verificationCode;
                existingCode.ExpiresAt = DateTime.UtcNow.AddHours(1);
                existingCode.CodeType = codeType;
                existingCode.Email = email.ToLower();
            }
            else if (existingCode == null)
            {
                await _unitOfWork.VerificationCodeRepository.AddAsync(verificationEntity);
            }
            var subject = "Your Verification Code";
            var body = $"<html><body><h1>Hello,</h1><p>Your verification code is: <strong>{verificationCode}</strong></p></body></html>";

            await SendEmailAsync(email, subject, body);

            return verificationCode;
        }
    }
}
