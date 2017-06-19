using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using PagedList;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.Interfaces;
using PhotoAlbum.WEB.Models;

namespace PhotoAlbum.WEB.Controllers
{
    public class HomeController : Controller
    {
        private const string DefaultImageName = @"~/Images/default-user.png";

        private static IUserService _userService;
        private static IPictureService _pictureService;
        private static IUnitOfWork _unitOfWork;

        public HomeController() : this(_userService, _pictureService, _unitOfWork)
        {
        }

        public HomeController(IUserService userService, IPictureService pictureService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _pictureService = pictureService;
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Register", "Account");
            if (User.IsInRole("user"))
                return RedirectToAction("UserProfile", new {userId = User.Identity.GetUserId<int>()});
            if (User.IsInRole("admin"))
                return RedirectToAction("GetBanList", "Pictures");
            return View();
        }

        [Authorize]
        public ActionResult UserProfile(int? page, int? pictureId, int? userId)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Register", "Account");
            if (userId == null)
                return HttpNotFound();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            var userDto = _userService.FindById(userId.Value);
            var imagesDto = _pictureService.GetPicturesForUser(userDto.Id);
            Mapper.Initialize(cfg => cfg.CreateMap<PictureDto, IndexPictureViewModel>());
            var images = Mapper.Map<IEnumerable<PictureDto>, List<IndexPictureViewModel>>(imagesDto);
            var imagesPagedList = images.ToPagedList(pageNumber, pageSize);

            Mapper.Initialize(cfg => cfg.CreateMap<UserDto, ProfileViewModel>()
                .ForMember("Name", opt => opt.MapFrom(c => $"{c.FirstName} {c.LastName}"))
                .ForMember("Avatar", opt => opt.MapFrom(c => userDto.AvatarId == null
                    ? System.IO.File.ReadAllBytes(Server.MapPath(DefaultImageName))
                    : _pictureService.FindById(userDto.AvatarId.Value).Image))
                .ForMember("Photos", opt => opt.MapFrom(c => imagesPagedList))
                .ForMember("HasAvatar", opt => opt.MapFrom(c => c.AvatarId != null))
            );
            var user = Mapper.Map<UserDto, ProfileViewModel>(userDto);

            if (!ReferenceEquals(pictureId, null) && pictureId != 0)
            {
                var photoDto = _pictureService.FindById(pictureId.Value);
                Mapper.Initialize(cfg => cfg.CreateMap<PictureDto, DetailsPictureViewModel>()
                    .ForMember("IsLiked", opt => opt.MapFrom(c => _pictureService
                        .GetUsersWhoLikedIds(photoDto.Id)
                        .Contains(User.Identity.GetUserId<int>())))
                );
                var photo = Mapper.Map<PictureDto, DetailsPictureViewModel>(photoDto);

                ViewBag.Model = photo;
                ViewBag.Page = page;
            }

            return View(user);
        }

        [Authorize (Roles = "user")]
        public ActionResult SetProfilePhoto(int? id, int? pageNumber)
        {
            var user = _userService.FindById(User.Identity.GetUserId<int>());
            user.AvatarId = id;
            _userService.Update(user);
            _unitOfWork.Save();
            return RedirectToAction("UserProfile", new { pictureId = 0, userId = user.Id, page = pageNumber });
        }

        [Authorize]
        public ActionResult DeletePhoto(int? id, int? pageNumber, int uploaderId, bool returnToBanList)
        {
            _pictureService.Delete(id);
            _unitOfWork.Save();
            return returnToBanList 
                ? RedirectToAction("GetBanList", "Pictures", new{page = pageNumber}) 
                : RedirectToAction("UserProfile", new { pictureId = 0, userId = uploaderId, page = pageNumber});
        }

        public ActionResult Search(int? page, string searchString)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var usersDto = _userService.FindByName(searchString.ToLower())
                .Where(u => u.Id != User.Identity.GetUserId<int>() && u.Id != 1);

            Mapper.Initialize(cfg => cfg.CreateMap<UserDto, SearchUserModel>()
                .ForMember("Name", opt => opt.MapFrom(c => $"{c.FirstName} {c.LastName}"))
                .ForMember("Photo", opt => opt.MapFrom(c => c.AvatarId == null
                    ? System.IO.File.ReadAllBytes(Server.MapPath(DefaultImageName))
                    : _pictureService.FindById(c.AvatarId.Value).Image))
            );
            var users = Mapper.Map<IEnumerable<UserDto>, List<SearchUserModel>>(usersDto);

            ViewBag.SearchString = searchString;
            return View("SearchResult", users.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Autocomplete(string userName)
        {
            var userNames = _userService.FindByName(userName.ToLower()).Select(u => $"{u.FirstName} {u.LastName}").ToArray();
            var filteredNames = userNames.Where(u =>
                u.IndexOf(userName, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredNames, JsonRequestBehavior.AllowGet);

        }
    }
}