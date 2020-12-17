using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using Coza.Models.ViewModels;
using Coza.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Coza.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + ","+ SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> allProduct = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(allProduct);
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(s => new SelectListItem { 
                    Text = s.Name,
                    Value = s.Id.ToString()
                })
                
            };

            if(id == null)
            {
                // create new product
                return View(productViewModel);
            }

            productViewModel.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if(productViewModel.Product == null)
            {
                return NotFound();
            }

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                // check if a file was selected
                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productViewModel.Product.ImageUrl != null)
                    {
                        // for update, image url is not null
                        var imagePath = Path.Combine(webRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    productViewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // no image was selected

                    // for update
                    if(productViewModel.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productViewModel.Product.Id);
                        // get image from db and assign to view model
                        productViewModel.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                // create new product if Id == 0
                if(productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                else
                {
                    // update product
                    _unitOfWork.Product.Update(productViewModel.Product);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // load ProductViewModel and pass to view
               
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });

                if(productViewModel.Product.Id != 0)
                {
                    // for update
                    productViewModel.Product = _unitOfWork.Product.Get(productViewModel.Product.Id);
                }
            }

            return View(productViewModel);
        }

        #region API Calls
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);

            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Record Deleted Successfully" });
        }
        #endregion
    }
}
