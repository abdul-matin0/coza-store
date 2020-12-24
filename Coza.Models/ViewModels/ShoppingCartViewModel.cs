using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<Product> ProductList { get; set; }
        public ShoppingCart CartObj { get; set; }
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
