using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coza.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            //get logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // display list of all products in cart based on user
            ShoppingCartViewModel shoppingCartVM = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUser.Id == claims.Value, includeProperties: "Product"),
                OrderHeader = new Models.OrderHeader()
            };

            shoppingCartVM.OrderHeader.OrderTotal = 0;

            foreach(var items in shoppingCartVM.ListCart)
            {
                items.Price = (items.Product.Price * items.Count);
                shoppingCartVM.OrderHeader.OrderTotal += (items.Price);

            }

            return View(shoppingCartVM);
        }
    }
}
