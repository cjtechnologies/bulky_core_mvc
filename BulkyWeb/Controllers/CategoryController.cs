using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) 
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _db.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            Console.WriteLine("name: {0}",obj.Name);
            Console.WriteLine("display order: {0}", obj.DisplayOrder);
            //if (obj.DisplayOrder == 0)
            //{
            //    ModelState.AddModelError("DisplayOrder", "DisplayOrder must not be 0 or empty");
            //}
            //if (obj.Name != null && obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name","Name & DisplayOrder must not be same");
            //}
            //if (obj.Name!=null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "Name must not be test");
            //}
            if (ModelState.IsValid) 
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();            
        }
    }
}
