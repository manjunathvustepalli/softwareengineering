using System.ComponentModel.DataAnnotations;

namespace MyBackendApi.Models
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}