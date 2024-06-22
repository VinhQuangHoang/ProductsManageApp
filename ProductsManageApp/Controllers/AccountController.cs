using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AccountController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

            // Đảm bảo returnUrl là một URL hợp lệ
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action(nameof(HomeController.Index), "Home");
            }

            // Đưa returnUrl vào ViewData để sử dụng trong View
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
                return RedirectToAction(nameof(HomeController.Index), "adminHome", new { area = "Admin" });
            }
            else
            {
                //return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                    //return RedirectToAction(nameof(AccountController.AccessDenied), "Account");
                else
                    return RedirectToAction(nameof(HomeController.Index), "Home");
            }
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

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}

