using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.EF.Identity;

namespace PhotoAlbum.DAL.EF
{
    public class PhotoAlbumContext : IdentityDbContext<User,
        CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public PhotoAlbumContext() : base("name=PhotoAlbumDb")
        {
            Database.SetInitializer<PhotoAlbumContext>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public PhotoAlbumContext(string conectionString) : base(conectionString) { }

        public static PhotoAlbumContext Create()
        {
            return new PhotoAlbumContext();
        }

        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<ExceptionDetail> ExceptionDetails { get; set; }

        public DbSet<CustomUserLogin> UserLogins { get; set; }
        public DbSet<CustomUserClaim> UserClaims { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomUserLogin>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<CustomUserClaim>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<CustomUserClaim>().Property(u => u.ClaimType).HasMaxLength(150);
            modelBuilder.Entity<CustomUserClaim>().Property(u => u.ClaimValue).HasMaxLength(500);

            modelBuilder.Entity<Picture>()
                .HasRequired(c => c.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasRequired(c => c.Pictures)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);
        }
    }
}
