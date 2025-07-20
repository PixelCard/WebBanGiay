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

            // Kiểm tra người dùng có chọn giao hàng tận nơi không
            bool isShipping = !string.IsNullOrEmpty(form["shipping"]);

            // Lấy ghi chú đơn hàng (order notes)
            string notes = string.IsNullOrWhiteSpace(form["orderNotes"]) ? null : form["orderNotes"].Trim();

            int? shippingId = null;
            if (isShipping)
            {
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
                shippingId = shipping.ShippingID;
            }

            // Tạo order mới
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                Status = "pending",
                SubTotal = cart.Sum(x => x.TotalPrice),
                TotalAmount = cart.Sum(x => x.TotalPrice) + 25.00m,
                ShippingID = shippingId,
                PaymentMethodID = paymentMethodId,
                PaymentStatus = "Pending",
                OrderNumber = Guid.NewGuid().ToString(),
                Notes = notes
            };

            try
            {
                db.Orders.Add(order);
                db.SaveChanges();

                // Thêm chi tiết đơn hàng cho từng sản phẩm trong giỏ hàng
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        VariantID = item.VariantID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price, // Giá tại thời điểm đặt hàng
                        TotalPrice = item.TotalPrice // Tổng tiền cho sản phẩm này
                    };
                    db.OrderDetails.Add(orderDetail);
                }
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return View("Error"); 
            }

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            // Chuyển hướng tới trang cảm ơn hoặc đơn hàng thành công   
            return RedirectToAction("Index", "Home");
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