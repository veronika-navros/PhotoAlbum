using PhotoAlbum.BLL.Interface.DTO;

namespace PhotoAlbum.BLL.Interface.Services
{
    public interface ILikeService
    {
        bool Create(LikeDto like);
        bool Delete(int userId, int pictureId);
    }
}
