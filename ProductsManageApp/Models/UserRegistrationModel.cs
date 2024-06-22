using System.ComponentModel.DataAnnotations;

namespace ProductsManageApp.Models
{
    public class UserRegistrationModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserName is required!")]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Password requires upper and lower case special characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
