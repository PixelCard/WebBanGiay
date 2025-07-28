using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;
using System.Data.Entity;

namespace WebBanGiay.Controllers
{
    public class OrderAccountController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: OrderAccount
        public ActionResult OrderAccountPage()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["Email"] == null)
            {
                return RedirectToAction("LoginPage", "Account");
            }

            string userEmail = Session["Email"].ToString();
            
            // Tìm tài khoản của người dùng
            var account = db.Accounts.FirstOrDefault(a => a.Email == userEmail);
            if (account == null)
            {
                return RedirectToAction("LoginPage", "Account");
            }

            // Tìm khách hàng tương ứng
            var customer = db.Customers.FirstOrDefault(c => c.IDAccount == account.IDTK);
            if (customer == null)
            {
                return RedirectToAction("LoginPage", "Account");
            }

            // Lấy danh sách đơn hàng của khách hàng với các navigation properties
            var orders = db.Orders
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant))
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.Product))
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.Size))
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.Color))
                .Where(o => o.CustomerID == customer.CustomerID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.Customer = customer;
            ViewBag.Orders = orders;

            return View();
        }

        // POST: OrderAccount/CancelOrder
        [HttpPost]
        public JsonResult CancelOrder(int orderId)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (Session["Email"] == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
                }

                string userEmail = Session["Email"].ToString();
                
                // Tìm tài khoản của người dùng
                var account = db.Accounts.FirstOrDefault(a => a.Email == userEmail);
                if (account == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản." });
                }

                // Tìm khách hàng tương ứng
                var customer = db.Customers.FirstOrDefault(c => c.IDAccount == account.IDTK);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng." });
                }

                // Tìm đơn hàng cần hủy
                var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId && o.CustomerID == customer.CustomerID);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                // Kiểm tra trạng thái đơn hàng - chỉ cho phép hủy khi chưa giao hàng
                if (order.Status == "Confirmed" || order.Status == "Shipped")
                {
                    return Json(new { success = false, message = "Không thể hủy đơn hàng đã hoàn thành hoặc đang giao hàng." });
                }

                // Cập nhật trạng thái đơn hàng thành "Cancelled"
                order.Status = "Cancelled";
                order.OrderDate = DateTime.Now;

                // Nếu trạng thái thanh toán là "Paid" thì chuyển thành "Failed"
                if (order.PaymentStatus == "Paid" || order.PaymentStatus == "Pending")
                {
                    order.PaymentStatus = "Failed";
                }

                // Lưu thay đổi vào database
                db.SaveChanges();

                return Json(new { success = true, message = "Đã hủy đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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