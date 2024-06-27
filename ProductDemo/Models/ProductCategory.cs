using System.ComponentModel.DataAnnotations;

namespace ProductDemo.Models
{
    public class ProductCategory
    {
        [Key]
        public Guid ProductTypeId { get; set; }

        [MaxLength(100)]
        public string? ProductTypeName { get; set; }
    }
}
