using System.Net.Mail;
using System.Net;

namespace CodelineAirlines.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
         
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            { 
                var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("codelineairlines@gmail.com", "thgp ysew amvu eyrj"),
                    EnableSsl = true
                };

                var message = new MailMessage("codelineairlines@gmail.com", toEmail, subject, body);

                await client.SendMailAsync(message);

                _logger.LogInformation($"Email successfully sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                throw;
            }
        }




    }
}
