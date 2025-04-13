using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class UserRegisterBaseDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Linkedin { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public bool AllowSendingEmail { get; set; } = true;
 
    }
}
