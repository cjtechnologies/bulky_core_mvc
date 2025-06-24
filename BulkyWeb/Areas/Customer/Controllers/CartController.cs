using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
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
        [BindProperty]
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
                OrderHeader = new()
            };
            foreach(var cart in CartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart); 
                CartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(CartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appUser = _unitOfWork.UserRepo.Get(u => u.Id == userId);

            CartVM = new()
            {
                ShoppingCartList = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            CartVM.OrderHeader.ApplicationUser = appUser;
            CartVM.OrderHeader.Name = appUser.Name;
            CartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            CartVM.OrderHeader.StreetAddress = appUser.StreetAddress;
            CartVM.OrderHeader.City = appUser.City;
            CartVM.OrderHeader.State = appUser.State;
            CartVM.OrderHeader.PostalCode = appUser.PostalCode;

            foreach (var cart in CartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                CartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(CartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser appUser = _unitOfWork.UserRepo.Get(u => u.Id == userId);

            CartVM.ShoppingCartList = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            
            CartVM.OrderHeader.OrderDate = System.DateTime.Now;
            CartVM.OrderHeader.ApplicationUserId = userId;             

            foreach (var cart in CartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                CartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                // it is a regular customer 
                CartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                CartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // it is a company user
                CartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                CartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrdHeaderRepo.Add(CartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in CartVM.ShoppingCartList)
            {
                OrderDetail detail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = CartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrdDetailRepo.Add(detail);
                _unitOfWork.Save();
            }
            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                // it is a regular customer account and we need to capture payment
                // stripe logic
            }
            int OrderHeaderId=CartVM.OrderHeader.Id;
            //CartVM = new() { ShoppingCartList = [], OrderHeader = new() };
            return RedirectToAction(nameof(OrderConfirmation), new {id= OrderHeaderId });
        }

        public IActionResult OrderConfirmation(int id) 
        { 
            return View(id);
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
