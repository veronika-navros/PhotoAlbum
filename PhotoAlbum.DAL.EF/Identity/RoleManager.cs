using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoAlbum.DAL.EF.Entities;

namespace PhotoAlbum.DAL.EF.Identity
{
    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore<Role> store)
            : base(store)
        { }
    }
}
