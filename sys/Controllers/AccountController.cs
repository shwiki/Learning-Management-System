using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using sys.Models;

namespace sys.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController() { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await SignInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Check the role of the user and redirect accordingly
                    if (await UserManager.IsInRoleAsync(user.Id, "Teacher"))
                    {
                        // Redirect teacher to their dashboard
                        return RedirectToAction("Index", "TeacherDashboard");
                    }
                    else if (await UserManager.IsInRoleAsync(user.Id, "Student"))
                    {
                        // Redirect student to their dashboard
                        return RedirectToAction("Index", "StudentDashboard");
                    }
                    else if (await UserManager.IsInRoleAsync(user.Id, "Parent"))
                    {
                        // Redirect parent to their dashboard
                        return RedirectToAction("Index", "ParentDashboard");
                    }
                    else if (await UserManager.IsInRoleAsync(user.Id, "Admin"))
                    {
                        // Redirect parent to their dashboard
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        // If role is not recognized, you can redirect to a default page or show an error
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // ** Registration is restricted: only Admins and Teachers can invite users **
        // GET: /Account/Register
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Register()
        {
            // Optionally pass available roles for Teachers
            if (User.IsInRole("Teacher"))
            {
                ViewBag.AvailableRoles = new SelectList(new[] { "Parent", "Student" });
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string invitedRole)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            // Determine role assignment
            var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (await UserManager.IsInRoleAsync(currentUser.Id, "Admin"))
            {
                // Admin invites Teacher
                await UserManager.AddToRoleAsync(user.Id, "Teacher");
            }
            else if (await UserManager.IsInRoleAsync(currentUser.Id, "Teacher") &&
                     (invitedRole == "Parent" || invitedRole == "Student"))
            {
                // Teacher invites Parent or Student
                await UserManager.AddToRoleAsync(user.Id, invitedRole);
            }
            else
            {
                // Unauthorized or invalid role
                ModelState.AddModelError("", "You are not authorized to assign this role.");
                return View(model);
            }

            // Send reset-password link so the new user can set their own password
            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, code = token }, Request.Url.Scheme);
            // TODO: Send email with callbackUrl to model.Email

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        // Update the LogOff method to redirect to the home page instead of the login page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // Sign out the user
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Clear the user's session to ensure no lingering authentication data
            Session.Clear();

            // Redirect to the home page instead of the login page
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _signInManager?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // Updated ChallengeResult to implement ExecuteResult
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            private readonly string _provider;
            private readonly string _redirectUri;

            public ChallengeResult(string provider, string redirectUri)
            {
                _provider = provider;
                _redirectUri = redirectUri;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null) throw new ArgumentNullException(nameof(context));

                // 1) tell OWIN to challenge the external-login provider
                var props = new AuthenticationProperties { RedirectUri = _redirectUri };
                context.HttpContext
                       .GetOwinContext()
                       .Authentication
                       .Challenge(props, _provider);

                // 2) return 401 so the middleware can take over
                base.ExecuteResult(context);
            }
        }
        #endregion
    }
}
