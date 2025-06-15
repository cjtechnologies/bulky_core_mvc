using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)] // for whole controller
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> list = _unitOfWork.CompRepo.GetAll().ToList();
            
            return View(list);
        }

        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                // create
                return View(new Company());
            }
            else {
                //update
                Company? obj = _unitOfWork.CompRepo.Get(u => u.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                return View(obj);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company obj )
        {
            if (ModelState.IsValid) 
            {                
                if (obj.Id == 0)
                {
                    _unitOfWork.CompRepo.Add(obj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.CompRepo.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }
                _unitOfWork.Save();                
                return RedirectToAction("Index");
            } 
            else
            {
                return View(obj);            
            }
        }
 
        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> list = _unitOfWork.CompRepo.GetAll().ToList();
            return Json(new { data = list });
        }
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Missing id" });
            }
            Company? obj = _unitOfWork.CompRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return Json(new {success=false,message= "Error while deleting" });
            }            
            _unitOfWork.CompRepo.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Your record has been deleted." });
        }

        #endregion
    }
}
