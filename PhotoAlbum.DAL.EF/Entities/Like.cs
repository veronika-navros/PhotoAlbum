using System.ComponentModel.DataAnnotations;

namespace PhotoAlbum.DAL.EF.Entities
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public int PictureId { get; set; }
        public Picture Picture { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
