using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using System.Reflection.Emit;

namespace ProductDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProductDemo.Models.Product> Product { get; set; }
        public DbSet<ProductDemo.Models.ProductCategory> ProductCategory { get; set; }
        public DbSet<ProductDemo.Models.Invoice> Invoice { get; set; }
        public DbSet<ProductDemo.Models.CartItem> CartItem { get; set; }
    }
}
