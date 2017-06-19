using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoAlbum.DAL.EF.Identity;

namespace PhotoAlbum.DAL.EF.Entities
{
    public class User : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Info { get; set; }

        public bool IsBanned { get; set; }

        public bool PasswordIsClear { get; set; }

        public int? AvatarId { get; set; }
        public Picture Avatar { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<User> Subscribers { get; set; }

        public virtual ICollection<User> Subscriptions { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
