using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
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
            // this commented code is moved to ShoppingCartViewComponent file
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //if (claim != null)
            //{
            //    int cartItemCount = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == claim.Value).Count();
            //    HttpContext.Session.SetInt32(SD.SessionCart, cartItemCount);
            //}
            IEnumerable<Product> list = _unitOfWork.PdtRepo.GetAll(includeProperties: "Category,ProductImages");
            return View(list);
        }

        public IActionResult Details(int pdtId)
        {
            Product? pdt = _unitOfWork.PdtRepo.Get(u => u.Id == pdtId, includeProperties: "Category,ProductImages");
            if (pdt == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                ProductId = pdtId,
                Product = pdt,
                Count = 1
            };
            
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            cart.ApplicationUserId = userId!;

            ShoppingCart? dbCart = _unitOfWork.CartRepo.Get(u=>u.ApplicationUserId==userId! && u.ProductId==cart.ProductId);
            if (dbCart != null) 
            {
                // shopping cart exists
                dbCart.Count += cart.Count;
                _unitOfWork.CartRepo.Update(dbCart);
                _unitOfWork.Save();
            }
            else
            {
                // add cart record
                _unitOfWork.CartRepo.Add(cart);
                _unitOfWork.Save();
            }
            int cartItemCount = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == userId!).Count();
            HttpContext.Session.SetInt32(SD.SessionCart, cartItemCount);
            TempData["success"] = "Cart updated successfully";            
            return RedirectToAction(nameof(Index));
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
