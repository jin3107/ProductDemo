using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDemo.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime AddedDate { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public string? BuyerName { get; set; }
    }
}
