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
            return RedirectToAction(nameof(OrderConfirmation), new {id= OrderHeaderId, paymentStatus="paid" });
        }

        public IActionResult OrderConfirmation(int id, string paymentStatus) 
        {
            OrderHeader orderHeader = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                //var service = new SessionService();
                //Session session = service.Get(orderHeader.SessionId);

                //if (session.PaymentStatus.ToLower() == "paid")
                //{
                
                string sessionId = Guid.NewGuid().ToString();
                string sessionPaymentIntentId = Guid.NewGuid().ToString();
                _unitOfWork.OrdHeaderRepo.UpdateStripePaymentID(id, sessionId, sessionPaymentIntentId);
                    _unitOfWork.OrdHeaderRepo.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                //}
                HttpContext.Session.Clear();

            }

            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
            //    $"<p>New Order Created - {orderHeader.Id}</p>");

            List<ShoppingCart> shoppingCarts = _unitOfWork.CartRepo
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.CartRepo.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
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
            var cartFromDb = _unitOfWork.CartRepo.Get(u => u.Id == cartId, tracked: true);
            if (cartFromDb.Count <= 1)
            {
                // remove 
                int cartItemCount = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, cartItemCount);
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
            var cartFromDb = _unitOfWork.CartRepo.Get(u => u.Id == cartId, tracked: true);
            int cartItemCount = _unitOfWork.CartRepo.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, cartItemCount);
            
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
