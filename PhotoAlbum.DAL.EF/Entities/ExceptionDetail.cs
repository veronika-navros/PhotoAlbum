using System;
using System.ComponentModel.DataAnnotations;

namespace PhotoAlbum.DAL.EF.Entities
{
    public class ExceptionDetail
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string Url { get; set; }

        public string UrlReferrer { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string StackTrace { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
