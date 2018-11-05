using PandaWebApp.Models;
using PandaWebApp.Models.Enums;
using PandaWebApp.ViewModels.Users;
using SIS.HTTP.Cookies;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Services;
using System;
using System.Linq;

namespace PandaWebApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        public IHttpResponse Login()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(LoginInputModel model)
        {
            var username = model.Username;
            var password = model.Password;

            var hashedPassword = this.hashService.Hash(password);

            var user = this.Context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user == null)
            {
                return this.BadRequestErrorWithView("Invalid username or password.");
            }

            var mvcUser = new MvcUserInfo
            {
                Username = user.Username,
                Role = user.Role.ToString(),
                Info = user.Email,
            };
            var cookieContent = this.UserCookieService.GetUserCookie(mvcUser);

            var cookie = new HttpCookie(".auth-cakes", cookieContent, 7) { HttpOnly = true };
            this.Response.Cookies.Add(cookie);
            return this.Redirect("/");
        }

        public IHttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Register(RegisterInputModel model)
        {
            var username = model.Username;
            var password = model.Password;
            var confirmPassword = model.ConfirmPassword;
            var email = model.Email;

            if (password != confirmPassword)
            {
                return this.BadRequestErrorWithView("Passwords do not match!");
            }

            var hashedPassword = this.hashService.Hash(password);

            var user = new User
            {
                Username = username,
                Email = email,
                Password = hashedPassword,
                Role = Role.User
            };

            if (!this.Context.Users.Any())
            {
                user.Role = Role.Admin;
            }

            this.Context.Users.Add(user);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                return this.BadRequestErrorWithView(e.Message);
            }

            return this.Redirect("/users/login");
        }

        [Authorize]
        public IHttpResponse Logout()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return this.Redirect("/");
            }

            var cookie = this.Request.Cookies.GetCookie(".auth-cakes");
            cookie.Delete();
            this.Response.Cookies.Add(cookie);
            return this.Redirect("/");
        }
    }
}
