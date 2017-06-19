using System.Linq;
using AutoMapper;
using Ninject;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.BLL.Services
{
    public class LikeService : ILikeService
    {
        private readonly IRepository<Like> _likeRepository;

        [Inject]
        public LikeService(IRepository<Like> likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public bool Create(LikeDto like)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<LikeDto, Like>());
            _likeRepository.Create(Mapper.Map<LikeDto, Like>(like));
            return true;
        }

        public bool Delete(int userId, int pictureId)
        {
            var like = _likeRepository.Find(l => l.UserId == userId && l.PictureId == pictureId).FirstOrDefault();

            if (ReferenceEquals(like, null))
                return false;

            _likeRepository.Delete(like.Id);
            return true;
        }
    }
}
