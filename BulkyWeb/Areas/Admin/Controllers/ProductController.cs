using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) 
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> list = _unitOfWork.PdtRepo.GetAll(includeProperties: "Category").ToList();
            
            return View(list);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CatRepo
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                // create
                return View(productVM);
            }
            else {
                //update
                Product? obj = _unitOfWork.PdtRepo.Get(u => u.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                productVM.Product = obj;
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid) 
            {
                string wwwroot = _webHostEnvironment.WebRootPath;
                if (file != null )
                {
                    //string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwroot, @"images\product", file.FileName);
                    Console.WriteLine("productPath: {0}", productPath);
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        // delete the old image
                        var oldImagePath = Path.Combine(wwwroot, obj.Product.ImageUrl.TrimStart('/'));
                        Console.WriteLine("oldImagePath: {0}", oldImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var filestream = new FileStream(productPath, FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    obj.Product.ImageUrl = @"/images/product/" + file.FileName;
                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.PdtRepo.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.PdtRepo.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();                
                return RedirectToAction("Index");
            } else
            {
                obj.CategoryList = _unitOfWork.CatRepo
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);            
            }
        }
              
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? obj = _unitOfWork.PdtRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? obj = _unitOfWork.PdtRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.PdtRepo.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
            
        }
    }
}
