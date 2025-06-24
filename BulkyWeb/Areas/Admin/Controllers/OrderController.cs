using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new() {
                OrderHeader = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrdDetailRepo.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDb = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name; 
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrdHeaderRepo.Update(orderHeaderFromDb); 
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrdHeaderRepo.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new {orderId=OrderVM.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrdHeaderRepo.Update(orderHeader);
            _unitOfWork.Save();
            
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                //var options = new RefundCreateOptions
                //{
                //    Reason = RefundReasons.RequestedByCustomer,
                //    PaymentIntent = orderHeader.PaymentIntentId
                //};
                //var service = new RefundService();
                //Refund refund = service.Create(options);

                _unitOfWork.OrdHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrdHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrdHeaderRepo
                .Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrdDetailRepo
                .GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            // stripe logic
            string sessionId = Guid.NewGuid().ToString();
            string sessionPaymentIntentId = Guid.NewGuid().ToString();
            _unitOfWork.OrdHeaderRepo.UpdateStripePaymentID(OrderVM.OrderHeader.Id, sessionId, sessionPaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", "/admin/order/PaymentConfirmation?orderHeaderId="+ OrderVM.OrderHeader.Id); // goto order successfull
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
           OrderHeader orderHeader = _unitOfWork.OrdHeaderRepo.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is an order by company

                //var service = new SessionService();
                //Session session = service.Get(orderHeader.SessionId);

                //if (session.PaymentStatus.ToLower() == "paid")
                //{
                    _unitOfWork.OrdHeaderRepo.UpdateStripePaymentID(orderHeaderId, orderHeader.SessionId, orderHeader.PaymentIntentId);
                    _unitOfWork.OrdHeaderRepo.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                //}
            }
            return View(orderHeaderId);
        }

        #region Api Calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> list;
            
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                list = _unitOfWork.OrdHeaderRepo.GetAll(includeProperties: "ApplicationUser").ToList();
            } else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity!;
                var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;                
                list = _unitOfWork.OrdHeaderRepo.GetAll(u=>u.ApplicationUserId==userId, includeProperties: "ApplicationUser").ToList();
            }

                switch (status)
                {
                    case "pending":
                        list = list.Where(u => u.OrderStatus == SD.StatusPending);
                        break;
                    case "inprocess":
                        list = list.Where(u => u.OrderStatus == SD.StatusInProcess);
                        break;
                    case "completed":
                        list = list.Where(u => u.OrderStatus == SD.StatusShipped);
                        break;
                    case "approved":
                        list = list.Where(u => u.OrderStatus == SD.StatusApproved);
                        break;
                    default:
                        break;
                }
            return Json(new { data = list });
        }

        #endregion
    }
}
