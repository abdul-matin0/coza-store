using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
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

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

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
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties: "Product"),
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

        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // display list of all products in cart based on user
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new Models.OrderHeader()
            };

            // get details of logged in user and map to OrderHeader properties
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claims.Value);

            foreach (var items in ShoppingCartVM.ListCart)
            {
                items.Price = (items.Product.Price * items.Count);
                ShoppingCartVM.OrderHeader.OrderTotal += (items.Price);

            }

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.Email = ShoppingCartVM.OrderHeader.ApplicationUser.Email;

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public IActionResult CheckoutPost([FromBody]Payment paymentData) {

            if(paymentData.Status.Equals("successful"))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartVM = new ShoppingCartViewModel
                {
                    OrderHeader = new OrderHeader(),
                    ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties: "Product")
                };


                ShoppingCartVM.OrderHeader.Email = paymentData.Email;
                ShoppingCartVM.OrderHeader.Name = paymentData.Name;
                ShoppingCartVM.OrderHeader.PhoneNumber = paymentData.PhoneNumber;
                ShoppingCartVM.OrderHeader.City = paymentData.City;
                ShoppingCartVM.OrderHeader.StreetAddress = paymentData.StreetAddress;
                ShoppingCartVM.OrderHeader.State = paymentData.State;
                ShoppingCartVM.OrderHeader.TransactionId = paymentData.TransactionId.ToString();
                ShoppingCartVM.OrderHeader.TxRef = paymentData.TxRef;
                ShoppingCartVM.OrderHeader.FlwRef = paymentData.FlwRef;

                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claims.Value);
                paymentData.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claims.Value);
                paymentData.ApplicationUserId = claims.Value;

                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                ShoppingCartVM.OrderHeader.PaymentStatus = paymentData.Status;
                ShoppingCartVM.OrderHeader.OrderTotal = paymentData.Amount;     // order total == total amount paid 

                _unitOfWork.Payment.Add(paymentData);

                _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var item in ShoppingCartVM.ListCart)
                {
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        ProductId = item.ProductId,
                        OrderId = ShoppingCartVM.OrderHeader.Id,
                        Price = paymentData.Amount,
                        Count = item.Count
                    };

                    _unitOfWork.OrderDetails.Add(orderDetails);
                }

                _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
                _unitOfWork.Save();
                // empty all items in shopping cart

                // set session 
                HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);

                return Json(new { success = true, message = "Payment Successful", url = Url.Action("Index", "Home") });
            }
            // something went wrong
            return RedirectToAction("Checkout");
            
        }
    }
}
