using Apparcus.Models;
using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Apparcus.Controllers
{
    public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, IEmailTemplateService emailTemplateService) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUserHelper _userHelper = userHelper;
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(string email, string password)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return ResponseHelper.JsonError("Fill form correctly");
            }
            var user = await _userHelper.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                return ResponseHelper.JsonError("Invalid detail or account does not exist, contact your Admin");
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return ResponseHelper.JsonError("Invalid user name or password");
            }
            user.Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            user.UserRole = user.Roles.Contains(Constants.SuperAdminRole) ? Constants.SuperAdminRole :
                            user.Roles.Contains(Constants.AdminRole) ? Constants.AdminRole :
                            Constants.UserRole;
            var url = _userHelper.GetValidatedUrl(user.Roles);
            return ResponseHelper.JsonSuccessWithReturnUrl(url);
        }

        [HttpPost]
        public async Task<JsonResult> Register(string userData)
        {
            if (string.IsNullOrEmpty(userData))
            {
                return ResponseHelper.ErrorMsg();
            }
            var applicationUser = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userData);
            if (applicationUser == null)
            {
                return ResponseHelper.ErrorMsg();
            }
            var user = await _userHelper.FindByEmailAsync(applicationUser.Email).ConfigureAwait(false);
            if (user != null)
            {
                return ResponseHelper.JsonError("Email already exists, please use another email");
            }
            var createStaff = await _userHelper.RegisterUser(applicationUser).ConfigureAwait(false);
            if (createStaff == null)
            {
                return ResponseHelper.JsonError("Unable to add");
            }
            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            _emailTemplateService.SendRegistrationEmail(user, baseUrl);
            return ResponseHelper.JsonSuccess("Added successfully");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        
    }
}
