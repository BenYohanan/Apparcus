using Core.Models;
using Logic.IHelpers;
using Microsoft.Extensions.Configuration;

namespace Logic.Helpers
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _emailConfiguration;
        private readonly string supportEmail = string.Empty;

        public EmailTemplateService(IEmailService emailService, IConfiguration configuration)
        {
            _emailConfiguration = configuration;
            _emailService = emailService;
            supportEmail = _emailConfiguration["EmailConfiguration:CompanyEmail"] ?? "okoronkwomarvelous@hotmail.com";
        }


        public bool SendRegistrationEmail(ApplicationUser user, string baseUrl)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return false;
            }
            string loginLink = $"{baseUrl}/Account/Login";
            string subject = "Welcome to Apparcus!";
            string message = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                    <div style='background-color: #ffffff; padding: 20px; border: 1px solid #e0e0e0; text-align: center;'>
                        <h1 style='color: #004aad; font-size: 24px;'>Welcome!</h1>
                        <p style='color: #333333; font-size: 16px;'>
                            Dear {user.FullName},<br/>
                            Your account has been successfully created,<br/>
                            and you now have full access to Apparcus platform. <br/>
                            Please click the button below to log in and access your dashboard.
                        </p>
                        <p style='color: #333333;'>
                            Login Link: <a href='{loginLink}' style='color: #004aad;'>Login</a>.
                        </p>
                        <p style='color: #333333;'>
                            Need help? Contact us at <a href='mailto:{supportEmail}' style='color: #004aad;'>{supportEmail}</a>.
                        </p>
                        <p><b>Kind regards,</b><br/>Apparcus Team</p>
                    </div>
                </div>";

            try
            {
                _emailService.CallHangfire(user.Email, subject, message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
