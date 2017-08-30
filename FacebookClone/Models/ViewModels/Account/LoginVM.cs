using System.ComponentModel.DataAnnotations;

namespace FacebookClone.Models.ViewModels.Account
{
    public class LoginVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}