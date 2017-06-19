using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ninject;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.BLL.Services
{
    public class PictureService : IPictureService
    {
        private readonly IRepository<Picture> _pictureRepository;

        [Inject]
        public PictureService(IRepository<Picture> pictureRepository)
        {
            _pictureRepository = pictureRepository;
        }

        public bool Create(PictureDto picture)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<PictureDto, Picture>());
            _pictureRepository.Create(Mapper.Map<PictureDto, Picture>(picture));
            return true;
        }

        public int? FindByBytes(byte[] pictureDto)
        {
            var pictures = _pictureRepository.GetAll().ToList();
            foreach (var picture in pictures)
            {
                if (picture.Image.Length != pictureDto.Length)
                    continue;
                for (int i = 0; i < pictureDto.Length; i++)
                {
                    if (pictureDto[i] != picture.Image[i])
                        break;
                    if (i == pictureDto.Length - 1)
                        return picture.Id;
                }
            }
            return null;
        }

        public PictureDto FindById(int id)
        {
            Picture picture = _pictureRepository.Get(id);

            if (!ReferenceEquals(picture, null))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<Picture, PictureDto>());
                return Mapper.Map<Picture, PictureDto>(picture);
            }

            return null;
        }

        public void Update(PictureDto pictureDto)
        {
            var picture = _pictureRepository.Get(pictureDto.Id);
            if (!ReferenceEquals(picture, null))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<UserDto, User>()
                    .IgnoreAllSourcePropertiesWithAnInaccessibleSetter()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter());
                Mapper.Map(pictureDto, picture);
                _pictureRepository.Update(picture);
            }
        }

        public bool Delete(int? id)
        {
            if (ReferenceEquals(id, null))
                return false;

            _pictureRepository.Delete(id.Value);
            return true;
        }

        public IEnumerable<PictureDto> GetPicturesForUser(int? userId)
        {
            if (ReferenceEquals(userId, null))
                return null;

            var pictures = _pictureRepository.GetAll().Where(p => p.UserId == userId).ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<Picture, PictureDto>());
            return Mapper.Map<IEnumerable<Picture>, List<PictureDto>>(pictures);
        }

        public IEnumerable<int> GetUsersWhoLikedIds(int pictureId)
        {
            var picture = _pictureRepository.GetAll().FirstOrDefault(p => p.Id == pictureId);

            if (ReferenceEquals(picture, null))
                return null;

            return picture.Likes.Select(l => l.UserId);
        }

        public IEnumerable<PictureDto> GetComplainedAboutPictures()
        {
            var pictures = _pictureRepository.GetAll().Where(p => p.IsComplainedAbout).ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<Picture, PictureDto>());
            return Mapper.Map<IEnumerable<Picture>, List<PictureDto>>(pictures);
        }

        public void LikeDislike(int id, bool like)
        {
            var picture = _pictureRepository.Get(id);
            if (like)
                picture.LikesAmount++;
            else
                picture.LikesAmount--;
            _pictureRepository.Update(picture);
        }

        public void BanUnban(int id, bool ban)
        {
            var picture = _pictureRepository.Get(id);
            if (ban)
                picture.IsComplainedAbout = true;
            else
                picture.IsComplainedAbout = false;
            _pictureRepository.Update(picture);
        }
    }
}
