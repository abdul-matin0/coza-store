using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models.ViewModels;
using Coza.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coza.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public IActionResult Plus(int id) {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, includeProperties: "Product");
            cartItem.Count += 1;

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, includeProperties: "Product");

            if(cartItem.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartItem);
                int currentNumOfItem = (int)HttpContext.Session.GetInt32(SD.ssShoppingCart);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, currentNumOfItem - 1);
            }
            else
            {
                cartItem.Count -= 1;

            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id, includeProperties: "Product");

            _unitOfWork.ShoppingCart.Remove(cartItem);
            _unitOfWork.Save();

            int currentNumOfItem = (int)HttpContext.Session.GetInt32(SD.ssShoppingCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCart, currentNumOfItem - 1);

            return RedirectToAction(nameof(Index));
        }
    }
}
