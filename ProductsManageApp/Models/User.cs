using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProductsManageApp.Models
{
    public class User : IdentityUser
    {

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "PasswordHash is required")]
        public override string PasswordHash { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid PhoneNumber")]
        public string PhoneNumber { get; set; }

        public string? Role { get; set; }
        public string? Salt { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
