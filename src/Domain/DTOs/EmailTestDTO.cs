using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class EmailTestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
