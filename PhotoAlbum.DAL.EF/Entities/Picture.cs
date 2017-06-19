using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoAlbum.DAL.EF.Entities
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public int LikesAmount { get; set; }

        public bool IsComplainedAbout { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
    }
}
