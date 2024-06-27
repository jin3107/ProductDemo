using ProductDemo.Models;
using System.Collections.Generic;

namespace ProductDemo.ViewModels
{
    public class CategoryViewModel
    {
        public List<ProductCategory>? Categories { get; set; }
        public int Page { get; set; }
        public int NoOfPages { get; set; }
        public string? Search { get; set; }
    }
}
