using System.ComponentModel.DataAnnotations;

namespace AspNetCoreExamples.ViewModels.Account
{
    public class CreateAccountViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
