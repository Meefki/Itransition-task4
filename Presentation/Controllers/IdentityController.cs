using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Presentation.Models;
using Application.UseCases;
using Domain;
using Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Presentation.Helpers;

namespace Presentation.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public IdentityController(
            ILogger<IdentityController> logger, 
            IUserService userService,
            IUserRepository userRepository)
        {
            _logger = logger;
            _userService = userService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Login(LoginViewModel loginModel, string idaction)
        {
            //ClaimsPrincipal claimUser = HttpContext.User;
            //Claim? claim = claimUser.FindFirst(ClaimTypes.NameIdentifier);
            User? user = await IdentityHelper.GetAuthenticatedUserAsync(HttpContext, _userRepository);
            //if (claim is not null)
            //    user = await _userRepository.GetByIdAsync(Guid.Parse(claim!.Value));

            if (user is not null && !user.IsBlocked)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            loginModel.Action = idaction switch
            {
                "login" => IdentityAction.Login,
                "register" => IdentityAction.Register,
                _ => IdentityAction.Login
            };

            return View(nameof(IdentityController.Login), loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            try
            {
                await _userService.Login(loginVM.Email, loginVM.Password);
                var user = await _userRepository.GetByEmailAsync(new(loginVM.Email));
                await SingInAsync(user!.Id.ToString(), user.Email.Value, loginVM.KeepLoggedIn);
                //List<Claim> claims = new()
                //{
                //    new Claim(ClaimTypes.Email, loginVM.Email),
                //    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString())
                //};
                //ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //AuthenticationProperties properties = new()
                //{
                //    AllowRefresh = true,
                //    IsPersistent = loginVM.KeepLoggedIn,
                //};
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new(claimsIdentity), properties);
            }
            catch (Exception ex) // There is should be catching domain and common exceptions separately...
            {
                _logger.LogError(ex, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
                return View(loginVM);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginViewModel loginVM)
        {
            try
            {
                User user = new(Guid.NewGuid(), loginVM.Name ?? string.Empty, loginVM.Password, new(loginVM.Email));
                await _userService.Register(user);
                await SingInAsync(user.Id.ToString(), user.Email.Value, loginVM.KeepLoggedIn);

                //List<Claim> claims = new()
                //{
                //    new Claim(ClaimTypes.Email, user.Email.Value),
                //    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString())
                //};
                //ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //AuthenticationProperties properties = new()
                //{
                //    AllowRefresh = true,
                //    IsPersistent = loginVM.KeepLoggedIn,
                //};
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new(claimsIdentity), properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
                return View(loginVM);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize(Policy = "Default")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(IdentityController.Login), "Identity");
        }

        private async Task SingInAsync(string id, string email, bool keepLoggedIn)
        {
            List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, id)
                };
            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new()
            {
                AllowRefresh = true,
                IsPersistent = keepLoggedIn,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new(claimsIdentity), properties);
        }
    }
}
