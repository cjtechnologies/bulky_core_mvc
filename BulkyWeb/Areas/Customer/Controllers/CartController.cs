using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM CartVM { get; set; }
        
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            CartVM = new()
            {
                ShoppingCartList = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            };
            foreach(var cart in CartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart); 
                CartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(CartVM);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.CartRepo.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.CartRepo.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.CartRepo.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                // remove
                _unitOfWork.CartRepo.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.CartRepo.Update(cartFromDb);
            }                
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.CartRepo.Get(u => u.Id == cartId);
            _unitOfWork.CartRepo.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
