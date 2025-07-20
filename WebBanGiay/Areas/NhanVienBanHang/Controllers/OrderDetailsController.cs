using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;
using WebBanGiay.Filters;

namespace WebBanGiay.Areas.NhanVienBanHang.Controllers
{
    [NhanVienBanHangAuthorize]
    public class OrderDetailsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.ProductVariant);
            return View(orderDetails.ToList());
        }

        // GET: NhanVienBanHang/OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        //GET: NhanVienBanHang/OrderDetails/HistoryByCustomer/5
        public ActionResult HistoryByCustomer(int? customerId)
        {
            if (customerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy tất cả các OrderDetail của các đơn hàng thuộc khách hàng này
            var orderDetails = db.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.ProductVariant)
                .Where(od => od.Order.CustomerID == customerId)
                .OrderByDescending(od => od.Order.OrderDate)
                .ToList();

            ViewBag.Customer = db.Customers.Find(customerId);
            return View(orderDetails);
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
