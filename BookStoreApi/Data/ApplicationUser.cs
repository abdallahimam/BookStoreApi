using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Fullname { get; set; }
    }
}
