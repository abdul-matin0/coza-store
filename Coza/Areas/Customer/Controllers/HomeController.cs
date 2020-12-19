using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Coza.Models.ViewModels;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Coza.Utility;
using Microsoft.AspNetCore.Http;

namespace Coza.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // get products
            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductItems = _unitOfWork.Product.GetAll(),
                Category = _unitOfWork.Category.GetAll()
            };

            // get Id of logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // if value == null, user not logged in
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }

            return View(productViewModel);

        }

        public IActionResult Products()
        {
            // get products
            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductItems = _unitOfWork.Product.GetAll(),
                Category = _unitOfWork.Category.GetAll()
            };

            return View(productViewModel);
        }

        public IActionResult Details(int id)
        {

            Product product = new Product();
            product = _unitOfWork.Product.GetFirstOrDefault(m => m.Id == id, includeProperties: "Category");

            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };

            ShoppingCartViewModel cartVM = new ShoppingCartViewModel()
            {
                CartObj = cartObj,
                ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category")
            };

            return View(cartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCartViewModel cartVM)
        {

            if (ModelState.IsValid)
            {
                // get user identity
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartVM.CartObj.ApplicationUserId = claims.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ApplicationUserId == cartVM.CartObj.ApplicationUserId && u.ProductId == cartVM.CartObj.ProductId, 
                    includeProperties: "Product");

                if(cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(cartVM.CartObj);
                }
                else
                {
                    cartFromDb.Count += cartVM.CartObj.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }

                _unitOfWork.Save();
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartVM.CartObj.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction("Index");
            }
            else
            {
                // display details back to view
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(m => m.Id == cartVM.CartObj.Id, includeProperties: "Category");

                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };

                ShoppingCartViewModel cart = new ShoppingCartViewModel()
                {
                    CartObj = cartObj,
                    ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category")
                };

                return View(cart);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
