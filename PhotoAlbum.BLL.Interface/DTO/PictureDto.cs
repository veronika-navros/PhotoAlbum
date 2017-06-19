using System;

namespace PhotoAlbum.BLL.Interface.DTO
{
    public class PictureDto
    {
        public int Id { get; set; }

        public byte[] Image { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public int LikesAmount { get; set; }

        public bool IsComplainedAbout { get; set; }
    }
}
