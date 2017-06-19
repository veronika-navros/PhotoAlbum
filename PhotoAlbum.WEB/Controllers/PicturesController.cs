using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
    public class PicturesController : Controller
    {
        private static IPictureService _pictureService;
        private static ILikeService _likeService;
        private static IUnitOfWork _unitOfWork;

        public PicturesController() : this(_pictureService, _likeService, _unitOfWork)
        {
        }

        public PicturesController(IPictureService pictureService, ILikeService likeService, IUnitOfWork unitOfWork)
        {
            _pictureService = pictureService;
            _likeService = likeService;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "user")]
        public ActionResult Upload()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public ActionResult Save(HttpPostedFileBase loadImage, UploadPictureViewModel model)
        {
            if (!ReferenceEquals(loadImage, null))
            {
                using (var binaryReader = new BinaryReader(loadImage.InputStream))
                {
                    model.Image = binaryReader.ReadBytes(loadImage.ContentLength);
                }

                Mapper.Initialize(cfg => cfg.CreateMap<UploadPictureViewModel, PictureDto>()
                    .ForMember("UserId", opt => opt.MapFrom(c => User.Identity.GetUserId<int>()))
                    .ForMember("DateTime", opt => opt.MapFrom(c => DateTime.Now))
                    .ForMember("LikesAmount", opt => opt.MapFrom(c => 0))
                    .ForMember("IsComplainedAbout", opt => opt.MapFrom(c => false))
                );
                var picture = Mapper.Map<UploadPictureViewModel, PictureDto>(model);

                _pictureService.Create(picture);
                _unitOfWork.Save();
            }
            return RedirectToAction("UserProfile", "Home", new {userId = User.Identity.GetUserId<int>()});
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public ActionResult Like(string likeButton)
        {
            var splitResult = likeButton.Split(new[] { " " }, StringSplitOptions.None);
            int pictureId = Convert.ToInt32(splitResult[0]);
            bool isLiked = Convert.ToBoolean(splitResult[1]);

            if (isLiked)
                _likeService.Delete(User.Identity.GetUserId<int>(), pictureId);
            else
            {
                var like = new LikeViewModel
                {
                    PictureId = pictureId,
                    UserId = User.Identity.GetUserId<int>()
                };
                Mapper.Initialize(cfg => cfg.CreateMap<LikeViewModel, LikeDto>());
                _likeService.Create(Mapper.Map<LikeViewModel, LikeDto>(like));
            }
            _unitOfWork.Save();

            _pictureService.LikeDislike(pictureId, !isLiked);
            _unitOfWork.Save();

            var pictureDto = _pictureService.FindById(pictureId);
            Mapper.Initialize(cfg => cfg.CreateMap<PictureDto, DetailsPictureViewModel>()
                .ForMember("IsLiked", opt => opt.MapFrom(c => _pictureService
                    .GetUsersWhoLikedIds(pictureDto.Id)
                    .Contains(User.Identity.GetUserId<int>()))));
            var picture = Mapper.Map<PictureDto, DetailsPictureViewModel>(pictureDto);

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Home/LikePartial.cshtml", picture);

            ViewBag.Model = picture;
            return RedirectToAction("UserProfile", "Home", new { pictureid = picture.Id, userId = pictureDto.UserId });
        }

        [Authorize(Roles = "user")]
        public ActionResult Complain(int? pageNumber, int? pictureId, int? uploaderId)
        {
            if (pictureId == null)
                return RedirectToAction("UserProfile", "Home", new{page = pageNumber, userId = uploaderId});

            _pictureService.BanUnban(pictureId.Value, true);
            _unitOfWork.Save();
            return RedirectToAction("UserProfile", "Home", new { page = pageNumber, userId = uploaderId });
        }


        [Authorize(Roles = "admin")]
        public ActionResult GetBanList(int? page, int? pictureId)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            var picturesDto = _pictureService.GetComplainedAboutPictures().ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<PictureDto, IndexPictureViewModel>());
            var pictures = Mapper.Map<IEnumerable<PictureDto>, List<IndexPictureViewModel>>(picturesDto);

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
            }

            ViewBag.PageNumber = page;
            ViewBag.IsBanListPage = true;
            return View("BanList", pictures.ToPagedList(pageNumber, pageSize));
        }

        [Authorize (Roles = "admin")]
        public ActionResult RejectComplaint(int? pictureId, int? uploaderId, int? pageNumber, bool returnToBanList)
        {
            if (pictureId == null)
                return RedirectToAction("GetBanList");

            _pictureService.BanUnban(pictureId.Value, false);
            _unitOfWork.Save();

            return returnToBanList
                ? RedirectToAction("GetBanList", new { page = pageNumber })
                : RedirectToAction("UserProfile", "Home", new { pictureId = 0, userId = uploaderId, page = pageNumber });
        }
    }
}