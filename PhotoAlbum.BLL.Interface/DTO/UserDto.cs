namespace PhotoAlbum.BLL.Interface.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Info { get; set; }

        public bool IsBanned { get; set; }

        public string Role { get; set; }
        
        public bool PasswordIsClear { get; set; }

        public int? AvatarId { get; set; }
    }
}
