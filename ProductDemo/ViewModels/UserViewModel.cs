using System.ComponentModel.DataAnnotations;

namespace ProductDemo.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
