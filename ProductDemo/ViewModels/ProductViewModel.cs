using ProductDemo.Models;
using System.Collections.Generic;

namespace ProductDemo.ViewModels
{
    public class ProductViewModel
    {
        public List<Product>? Products { get; set; }
        public int Page { get; set; }
        public int NoOfPages { get; set; }
        public string? Search { get; set; }
    }
}
