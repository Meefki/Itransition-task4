using Application.Repositories;
using Application.UseCases;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers
{
    [Authorize(Policy = "Default")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IUserService userService,
            IUserRepository userRepository)
        {
            _logger = logger;
            _userService = userService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(HomeViewModel? homeVM = null, int? viewCount = null)
        {
            homeVM ??= new();
            viewCount ??= homeVM?.ViewCount;
            homeVM!.Users = 
                (await _userRepository.TakeUsers(viewCount!.Value, x => x.CreatedAt))
                .Select(x => 
                    new UserViewModel()
                    {
                        Id = x.Id.ToString(),
                        CreatedAt = x.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        LastLoginAt = x.LastLoginAt.ToString("dd.MM.yyyy HH:mm"),
                        Email = x.Email.Value,
                        Name = x.Name,
                        IsBlocked = x.IsBlocked,
                        IsSelected = false
                    }).ToList();
            homeVM.UsersCount = await _userRepository.GetUsersCount();

            var user = await IdentityHelper.GetAuthenticatedUserAsync(HttpContext, _userRepository);
            @ViewData["LoggedUserName"] = user?.Name;
            return View(homeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Block(HomeViewModel homeVM)
        {
            try
            {
                await _userService.Block(
                    homeVM.Users
                        .Where(u => u.IsSelected)
                        .Select(u => u.Id));

                //foreach (var userVM in homeVM.Users.Where(u => u.IsSelected))
                //{
                //    userVM.IsBlocked = true;
                //}

                User? user = await IdentityHelper.GetAuthenticatedUserAsync(HttpContext, _userRepository);
                if (user is null || user.IsBlocked)
                    return RedirectToAction(nameof(IdentityController.Login), "Identity");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(HomeViewModel homeVM)
        {
            try
            {
                await _userService.Unblock(
                    homeVM.Users
                        .Where(u => u.IsSelected)
                        .Select(u => u.Id));

                //foreach (var user in homeVM.Users.Where(u => u.IsSelected))
                //{
                //    user.IsBlocked = false;
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HomeViewModel homeVM)
        {
            try
            {
                var usersForDeleting = homeVM.Users
                        .Where(u => u.IsSelected)
                        .ToList();
                await _userService.Delete(usersForDeleting.Select(u => u.Id));

                foreach (var userVM in usersForDeleting)
                {
                    homeVM.Users.Remove(userVM);
                }

                User? user = await IdentityHelper.GetAuthenticatedUserAsync(HttpContext, _userRepository);
                if (user is null || user.IsBlocked)
                    return RedirectToAction(nameof(IdentityController.Login), "Identity");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(HomeController.Index));
        }

        public IActionResult ShowMore(HomeViewModel homeVM)
        {
            homeVM.ViewCount += homeVM.ViewCountStep;

            return RedirectToAction(nameof(HomeController.Index), homeVM.ViewCount);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}