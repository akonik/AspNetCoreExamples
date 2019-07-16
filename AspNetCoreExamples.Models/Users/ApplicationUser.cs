using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreExamples.Models.Users
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        public string Name => UserName;

        [Required]
        public DateTime LastLoginDate { get; set; }
    }
}
