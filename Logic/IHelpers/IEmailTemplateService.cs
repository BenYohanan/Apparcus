using Core.Models;

namespace Logic.IHelpers
{
    public interface IEmailTemplateService
    {
        bool SendRegistrationEmail(ApplicationUser user, string baseUrl);
    }
}
