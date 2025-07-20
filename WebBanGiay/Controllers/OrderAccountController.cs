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