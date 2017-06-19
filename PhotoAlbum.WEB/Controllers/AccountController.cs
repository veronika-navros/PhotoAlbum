using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CustomAuth.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.Interfaces;
using PhotoAlbum.WEB.Models;

namespace PhotoAlbum.WEB.Controllers
{
    public class AccountController : Controller
    {
        private static IUserService _userService;
        private static IUnitOfWork _unitOfWork;

        public AccountController() : this(_userService, _unitOfWork)
        {
        }

        public AccountController(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ActionResult Register()
        {
            if (Request.IsAuthenticated )
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (model.Captcha != (string)Session[CaptchaImage.CaptchaValueKey])
            {
                ModelState.AddModelError("Captcha", "Incorrect input.");
                return View(model);
            }

            if (!ReferenceEquals(_userService.GetByEmail(model.Email), null))
            {
                ModelState.AddModelError("", "User with such Email is already registered.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                Mapper.Initialize(cfg => cfg.CreateMap<RegisterViewModel, UserDto>()
                    .ForMember("IsBanned", opt => opt.MapFrom(c => false))
                    .ForMember("PasswordIsClear", opt => opt.MapFrom(c => false))
                    .ForMember("Role", opt => opt.MapFrom(c => "user")));

                UserDto userViewModel = Mapper.Map<RegisterViewModel, UserDto>(model);

                await _userService.Create(userViewModel);
                _unitOfWork.Save();

                ClaimsIdentity claim = await _userService.Authenticate(userViewModel);
                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, claim);

                return View("~/Views/Home/Index.cshtml");
            }

            return View(model);
        }

        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                UserDto userViewModel = new UserDto
                {
                    Email = model.Email,
                    Password = model.Password
                };
                ClaimsIdentity claim = await _userService.Authenticate(userViewModel);
                if (claim == null)
                    ModelState.AddModelError("", "Wrong login or password");
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Captcha()
        {
            Session[CaptchaImage.CaptchaValueKey] =
                new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString(CultureInfo.InvariantCulture);
            var ci = new CaptchaImage(Session[CaptchaImage.CaptchaValueKey].ToString(), 211, 50, "Helvetica");

            Response.Clear();
            Response.ContentType = "image/jpeg";

            ci.Image.Save(Response.OutputStream, ImageFormat.Jpeg);

            ci.Dispose();
            return null;
        }

        [Authorize]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}