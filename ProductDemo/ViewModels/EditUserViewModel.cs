using System.ComponentModel.DataAnnotations;

namespace ProductDemo.ViewModels
{
    public class EditUserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }
    }
}
