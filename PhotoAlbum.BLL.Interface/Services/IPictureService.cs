using System.Collections.Generic;
using PhotoAlbum.BLL.Interface.DTO;

namespace PhotoAlbum.BLL.Interface.Services
{
    public interface IPictureService
    {
        bool Create(PictureDto picture);
        int? FindByBytes(byte[] image);
        PictureDto FindById(int id);
        void Update(PictureDto pictureDto);
        bool Delete(int? id);
        IEnumerable<PictureDto> GetComplainedAboutPictures();
        IEnumerable<PictureDto> GetPicturesForUser(int? userId);
        IEnumerable<int> GetUsersWhoLikedIds(int pictureId);
        void LikeDislike(int id, bool like);
        void BanUnban(int id, bool ban);
    }
}
