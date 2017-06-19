using System;
using System.ComponentModel.DataAnnotations;

namespace PhotoAlbum.WEB.Models
{
    public class UploadPictureViewModel
    {
        public byte[] Image { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }

    public class IndexPictureViewModel
    {
        public int Id { get; set; }

        public byte[] Image { get; set; }
    }

    public class DetailsPictureViewModel
    {
        public int Id { get; set; }

        public byte[] Image { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public int LikesAmount { get; set; }

        public bool IsComplainedAbout { get; set; }

        public bool IsLiked { get; set; }
    }
}