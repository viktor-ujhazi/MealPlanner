using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountsService _accountsService;
        //private readonly ICyberSecurityProvider _cyberSecurity;
        public AccountController(IAccountsService accountsService/*, ICyberSecurityProvider cyberSecurity*/)
        {
            _accountsService = accountsService;
            //_cyberSecurity = cyberSecurity;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterModel register)
        {


            try
            {
                _accountsService.Register(register);
            }
            catch (Npgsql.PostgresException)
            {

                return Redirect("InvalidUsername");
            }

            return Redirect("Login");
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterShop()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegisterShop(RegisterShopModel register)
        {


            try
            {
                _accountsService.RegisterShop(register);
            }
            catch (Npgsql.PostgresException)
            {

                return Redirect("InvalidUsername");
            }

            return Redirect("Login");
        }

        public IActionResult InvalidUsername()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginModel login)
        {
            //if (!_cyberSecurity.IsValidUser(username, password))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            Account account = _accountsService.Login(login.Email, login.Password);
            if (account == null)
            {
                return Redirect("Login");
            }
            else
            {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim("Id", account.UserID.ToString()),
                        new Claim("Email", account.Email),
                        new Claim("Username", account.Username),
                        new Claim("Role", account.Role)
                    }, CookieAuthenticationDefaults.AuthenticationScheme)),
                    new AuthenticationProperties());
                return Redirect($"../Home/Index");
            }
                //var claims = new List<Claim> { new Claim(ClaimTypes.Name, login.Email) };

                //var claimsIdentity = new ClaimsIdentity(
                //    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //var authProperties = new AuthenticationProperties
                //{
                //    //AllowRefresh = <bool>,
                //    // Refreshing the authentication session should be allowed.

                //    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                //    // The time at which the authentication ticket expires. A 
                //    // value set here overrides the ExpireTimeSpan option of 
                //    // CookieAuthenticationOptions set with AddCookie.

                //    //IsPersistent = true,
                //    // Whether the authentication session is persisted across 
                //    // multiple requests. When used with cookies, controls
                //    // whether the cookie's lifetime is absolute (matching the
                //    // lifetime of the authentication ticket) or session-based.

                //    //IssuedUtc = <DateTimeOffset>,
                //    // The time at which the authentication ticket was issued.

                //    //RedirectUri = <string>
                //    // The full path or absolute URI to be used as an http 
                //    // redirect response value.
                //};


                //await HttpContext.SignInAsync(
                //CookieAuthenticationDefaults.AuthenticationScheme,
                //new ClaimsPrincipal(claimsIdentity),
                //authProperties);

                //return RedirectToAction("Index", "Home");
           
        }

        [HttpPost]
        
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}