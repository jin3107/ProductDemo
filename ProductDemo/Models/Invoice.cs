using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDemo.Models
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? BuyerName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ProductName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ProductCategoryName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public int Quantity { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public Guid? ProductCategoryId { get; set; }

        [ForeignKey("ProductCategoryId")]
        public ProductCategory? ProductCategory { get; set; }

        public string? BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public ApplicationUser? Buyer { get; set; }
    }
}
