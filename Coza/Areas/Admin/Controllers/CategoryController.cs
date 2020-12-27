using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using Coza.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Coza.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var allObjCategory = _unitOfWork.Category.GetAll();

            return View(allObjCategory);
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if(id == null)
            {
                // create category
                return View(category);
            }
            
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                // check if a file was selected
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\categories");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (category.ImageUrl != null)
                    {
                        // for update, image url is not null
                        var imagePath = Path.Combine(webRootPath, category.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    category.ImageUrl = @"\images\categories\" + fileName + extension;
                }
                else
                {
                    // no image was selected

                    // for update
                    if (category.Id != 0)
                    {
                        Category objFromDb = _unitOfWork.Category.Get(category.Id);
                        // get image from db and assign to view model
                        category.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll(){
            // retrun all category list in json format
            var allObjCategory = _unitOfWork.Category.GetAll();

            return Json(new { data = allObjCategory });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting"});
            }

            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Record Deleted Successfully" });
        }
        #endregion 
    }
}
