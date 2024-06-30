using ProductDemo.Models;

namespace ProductDemo.ViewModels
{
    public class CartViewModel
    {
        public Product? SelectedProduct { get; set; }
        public List<Product>? Product {  get; set; }
        public List<CartItem>? CartItem { get; set; }
    }
}
