using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using PhotoAlbum.BLL.Interface.DTO;

namespace PhotoAlbum.BLL.Interface.Services
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetAll();
        Task<bool> Create(UserDto user);
        Task<ClaimsIdentity> Authenticate(UserDto userDto);
        UserDto GetByEmail(string email);
        UserDto FindById(int id);
        IEnumerable<UserDto> FindByName(string name);
        void Update(UserDto userDto);
        //IEnumerable<UserDto> GetFollowers();
        //IEnumerable<UserDto> GetFollowing();
    }
}
