using AutoMapper;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using ProductsManageApp.Data;
using ProductsManageApp.Models;

namespace ProductsManageApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context, IEmailSender emailSender)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        private bool IsLocalUrl(string url)
        {
            return Url.IsLocalUrl(url);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin userModel, string returnUrl = null)
        {

            // make sủe url is valid
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action(nameof(HomeController.Index), "Home");
            }

            // implement ViewData using in view
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, false);
            if (result.Succeeded)
            {

                var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == userModel.Email);
                return await RedirectToLocal(returnUrl, user);
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        private async Task<IActionResult> RedirectToLocal(string returnUrl, User user)
        {
            //var isAdmin = await _context.Users.AnyAsync(m => m.Email == user.Email);
            var isAdmin = user.Role.Equals("Admin");
            if (isAdmin)
            {
                return RedirectToAction(nameof(AdminController.Index), "Admin");
            }
            else
            {
                //return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
                if (Url.IsLocalUrl(returnUrl))
                    //return Redirect(returnUrl);
                    return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
                else
                    return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Register(UserRegistrationModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var isEmailAlreadyExists = _userManager.Users.Any(x => x.Email == userModel.Email);
            if (isEmailAlreadyExists)
            {
                ModelState.AddModelError("Email", "Email already exists!");
                return View(userModel);
            }

            var user = _mapper.Map<User>(userModel);

            user.Salt = "";
            user.FullName = userModel.UserName;
            user.Role = user.Email.Equals("hoangvinhquanggame1@gmail.com") ? "Admin" : "Visitor";

            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(userModel);
            }
            var returnUrl = HttpContext.Request.Query["returnUrl"].FirstOrDefault();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

            var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null);
            await _emailSender.SendEmailAsync(message);

            //await _signInManager.SignInAsync(user, isPersistent: false);
            //return RedirectToAction(nameof(HomeController.Index), "Home");
            return RedirectToAction(nameof(RequireEmailConfirmation));
        }

        public IActionResult RequireEmailConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (token == null || email == null)
            {
                return View("Error");
            }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // autologin when confirm email success
                await _signInManager.SignInAsync(user, isPersistent: false);

                // redirect to Home
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (!ModelState.IsValid)
                return View(forgotPassword);
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == forgotPassword.Email);
            //var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
                return RedirectToAction("Index", "Home");
            //return RedirectToAction(nameof(ForgotPasswordConfirmation));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);

            var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);
            await _emailSender.SendEmailAsync(message);
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
            //return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == resetPassword.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }

            // autologin when confirm email success
            await _signInManager.SignInAsync(user, isPersistent: false);

            // redirect to Home
            //return RedirectToAction(nameof(HomeController.Index), "Home");
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

    }
}

