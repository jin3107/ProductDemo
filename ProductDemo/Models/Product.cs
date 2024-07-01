using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDemo.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [MaxLength(100)]
        public string? ProductName { get; set; }

        [MaxLength(100)]
        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image {  get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string? Manufactor { get; set; }

        [MaxLength(100)]
        public string? MadeIn { get; set; }

        [MaxLength(100)]
        public string? Material { get; set; }

        [ForeignKey("ProductTypeId")]
        public ProductCategory? ProductCategory { get; set; }

        public Guid? ProductTypeId { get; set; }

        public int RemainingNumber { get; set; }

        public string? Description { get; set; }
    }
}

