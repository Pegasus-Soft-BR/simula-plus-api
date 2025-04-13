using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class UserForgotMyPasswordDTO
    {
        [Required]
        public string Email { get; set; }
    }
}
