using System;

namespace PhotoAlbum.BLL.Interface.DTO
{
    public class ExceptionDetailDto
    {
        public Guid Guid { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public string UrlReferrere { get; set; }

        public DateTime Date { get; set; }

        public string StackTrace { get; set; }

        public int? UserId { get; set; }
    }
}
