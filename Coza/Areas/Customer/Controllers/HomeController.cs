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

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObj)
        {
            return RedirectToAction("Index");
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
