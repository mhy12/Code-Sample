using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroService.IdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;
using MicroService.IdentityServer.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.Events;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace MicroService.IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService _events;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        private string _frontEndUrl = "http://localhost:4200";

        public HomeController(IEventService events, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService identityServerInteractionService)
        {
            _events = events;
            _userManager = userManager;
            _signInManager = signInManager;
            _identityServerInteractionService = identityServerInteractionService;
        }

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(_frontEndUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [Route("Account/Login")]
        [HttpGet]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult LoginRedirect()
        {
            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    await OnSuccess(model);

                    return Redirect(_frontEndUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(HomeController.Index), new { returnUrl = Alerts.LOCKED_OUT });
                }
                else
                {
                    return RedirectToAction(nameof(HomeController.Index), new { returnUrl = Alerts.NOT_FOUND });
                }
            }

            return RedirectToAction(nameof(HomeController.Index), new { returnUrl = Alerts.INTERNAL_ERROR });
        }

        #endregion Login

        #region Logout

        [Route("Account/Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var ctx = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);

            var authenticationProperties = new AuthenticationProperties()
            {
                RedirectUri = ctx.PostLogoutRedirectUri
            };

            return SignOut(authenticationProperties);
        }

        #endregion Logout

        #region Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToLocal(returnUrl);
                }

                AddErrors(result);
            }

            return View(model);
        }

        #endregion

        #region Helpers

        public async Task<IActionResult> Error(string errorId)
        {
            ErrorMessage message = await _identityServerInteractionService.GetErrorContextAsync(errorId);

            return RedirectToAction(nameof(HomeController.Index), new { returnUrl = Alerts.INTERNAL_ERROR });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task OnSuccess(LoginViewModel model)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

            var claims = new List<Claim>();

            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value, claim.Issuer));
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);

            HttpContext.User = claimsPrincipal;

            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName.ToString()));
        }

        #endregion
    }
}
