using PagedList;

namespace PhotoAlbum.WEB.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

        public bool IsBanned { get; set; }

        public bool HasAvatar { get; set; }

        public byte[] Avatar { get; set; }

        public PagedList<IndexPictureViewModel> Photos { get; set; }

        //public int FollowersAmount { get; set; }

        //public int FollowingAmount { get; set; }
    }
}