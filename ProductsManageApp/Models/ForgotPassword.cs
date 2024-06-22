using System.ComponentModel.DataAnnotations;

namespace ProductsManageApp.Models
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
