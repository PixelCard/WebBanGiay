using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    public class CheckOutController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: CheckOut
        public ActionResult Checkoutpage()
        {
            // Check if user is logged in
            if (Session["UserID"] != null)
            {
                int userID = (int)Session["UserID"];
                
                // Get customer information
                var customer = db.Customers
                    .Include("Account")
                    .Where(c => c.IDAccount == userID && c.IsActive == true)
                    .FirstOrDefault();

                if (customer != null)
                {
                    ViewBag.Customer = customer;
                    ViewBag.IsLoggedIn = true;
                }
                else
                {
                    ViewBag.IsLoggedIn = false;
                }
            }
            else
            {
                ViewBag.IsLoggedIn = false;
            }

            // Get cart information
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem>;
            if (cart != null && cart.Any())
            {
                ViewBag.Cart = cart;
                ViewBag.CartTotal = cart.Sum(item => item.TotalPrice);
            }

            else
            {
                // Redirect to shopping cart if cart is empty
                return RedirectToAction("ShoppingCartPage", "ShoppingCart");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Checkoutpage(FormCollection form)
        {
            // Lấy customer từ session
            if (Session["UserID"] == null)
                return RedirectToAction("LoginPage", "Account");

            int userID = (int)Session["UserID"];
            var customer = db.Customers.FirstOrDefault(c => c.IDAccount == userID && c.IsActive == true);
            if (customer == null)
                return RedirectToAction("LoginPage", "Account");

            // Lấy giỏ hàng
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("ShoppingCartPage", "ShoppingCart");

            // Lấy phương thức thanh toán từ form
            string paymentMethod = form["paymentMethod"];
            int paymentMethodId = (paymentMethod == "cod") ? 1 : 2;

            // Tạo shipping mới
            var shipping = new Shipping
            {
                ShippingAddress = customer.Address,
                ShippingPhone = customer.Phone,
                ShippingName = customer.FullName,
                ShippingFee = 25.00m
            };
            db.Shippings.Add(shipping);
            db.SaveChanges();

            // Tạo order mới
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                Status = "pending",
                SubTotal = cart.Sum(x => x.TotalPrice),
                TotalAmount = cart.Sum(x => x.TotalPrice) + 25.00m,
                ShippingID = shipping.ShippingID,
                PaymentMethodID = paymentMethodId,
                PaymentStatus = "Process"
            };
            db.Orders.Add(order);
            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            // Chuyển hướng tới trang cảm ơn hoặc đơn hàng thành công
            return RedirectToAction("OrderSuccess");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}