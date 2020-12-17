using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using Coza.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coza.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                if(category.Id == 0)
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
