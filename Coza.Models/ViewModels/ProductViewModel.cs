using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Product> ProductItems { get; set; }
    }
}
