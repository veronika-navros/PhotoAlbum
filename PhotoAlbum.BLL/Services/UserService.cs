using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Ninject;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.EF.Identity;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserManager UserManager { get; }
        public RoleManager RoleManager { get; }

        [Inject]
        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
            PhotoAlbumContext db = new PhotoAlbumContext("PhotoAlbumDb");

            var provider = new DpapiDataProtectionProvider("Sample");
            UserManager = new UserManager(new UserStore<User,
                CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>(db));
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(provider.Create("EmailConfirmation"));
            RoleManager = new RoleManager(new RoleStore<Role>(db));
        }

        public async Task<bool> Create(UserDto userDto)
        {
            User user = await UserManager.FindByEmailAsync(userDto.Email);
            if (ReferenceEquals(user, null))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<UserDto, User>()
                    .ForMember("UserName", opt => opt.MapFrom(c => c.Email))
                );
                user = Mapper.Map<UserDto, User>(userDto);

                var result = await UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Any())
                    return false;

                await UserManager.AddToRoleAsync(user.Id, userDto.Role);
                return true;
            }

            return false;
        }

        public async Task<ClaimsIdentity> Authenticate(UserDto userDto)
        {
            ClaimsIdentity claim = null;
            User user = await UserManager.FindByEmailAsync(userDto.Email);

            if (!ReferenceEquals(user, null))
            {
                if (user.PasswordIsClear && user.PasswordHash.Equals(userDto.Password)) { }
                else if (!user.PasswordIsClear)
                    user = await UserManager.FindAsync(userDto.Email, userDto.Password);
                else user = null;
            }

            if (!ReferenceEquals(user, null))
            {
                claim = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
            }
            return claim;
        }

        public IEnumerable<UserDto> GetAll()
        {
            var users = _userRepository.GetAll().ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());
            return Mapper.Map<List<User>, List<UserDto>>(users);
        }

        public UserDto GetByEmail(string email)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email.Equals(email));

            if (ReferenceEquals(user, null))
                return null;

            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());
            return Mapper.Map<User, UserDto>(user);
        }

        public UserDto FindById(int id)
        {
            User user = UserManager.FindById(id);
            if (!ReferenceEquals(user, null))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());
                return Mapper.Map<User, UserDto>(user);
            }
            return null;
        }

        public IEnumerable<UserDto> FindByName(string name)
        {
            var users = _userRepository.Find(
                u => (u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(name.ToLower()) 
                     || (u.LastName.ToLower() + " " + u.FirstName.ToLower()).Contains(name.ToLower()));
            var uniqueUsers = users.Distinct();

            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());
            return Mapper.Map<IEnumerable<User>, List<UserDto>>(uniqueUsers);
        }

        public void Update(UserDto userDto)
        {
            var user = _userRepository.Get(userDto.Id);
            if (!ReferenceEquals(user, null))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<UserDto, User>()
                    .IgnoreAllSourcePropertiesWithAnInaccessibleSetter()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter());
                Mapper.Map(userDto, user);
                _userRepository.Update(user);
            }
        }
    }
}
