using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
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

        public IActionResult Index()
        {
            // view list of all products in cart
            _unitOfWork.ShoppingCart.GetAll();
            return View();
        }
    }
}
