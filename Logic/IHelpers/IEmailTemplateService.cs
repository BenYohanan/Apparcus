using Core.Models;

namespace Logic.IHelpers
{
    public interface IEmailTemplateService
    {
        bool SendRegistrationEmail(ApplicationUser user, string baseUrl);
        bool SendPasswordResetEmail(ApplicationUser user, string resetUrl);

    }
}
