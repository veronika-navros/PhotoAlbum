using System.ComponentModel.DataAnnotations;

namespace PhotoAlbum.WEB.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]{1,20}$", ErrorMessage = "Incorrect first name")]
        [StringLength(20, ErrorMessage = "The {0} must be from 1 to 20 characters long.", MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]{1,20}$", ErrorMessage = "Incorrect last name")]
        [StringLength(20, ErrorMessage = "The {0} must be from 1 to 20 characters long.", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} can't be longer than 100 characters.")]
        [Display(Name = "Information")]
        [DataType(DataType.MultilineText)]
        public string Info { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Field E-mail contains invalid E-mail address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(30, ErrorMessage = "The {0} must be from 8 to 30 characters long.", MinimumLength = 8)]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", 
            ErrorMessage = "Field Password may contain lowercase and uppercase Latin letters, digits and special characters")]
        [DataType(DataType.Password, ErrorMessage = "Field Password contains invalid characters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessage = "Field Confirm Password contains invalid characters")]
        [StringLength(30, ErrorMessage = "The {0} must be from 8 to 30 characters long.", MinimumLength = 8)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Enter the code from the image")]
        public string Captcha { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Field E-mail contains invalid E-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Field Password contains invalid characters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}