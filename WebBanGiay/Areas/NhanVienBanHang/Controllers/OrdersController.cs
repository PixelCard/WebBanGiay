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
using WebBanGiay.helper;

namespace WebBanGiay.Areas.NhanVienBanHang.Controllers
{
    [NhanVienBanHangAuthorize]
    public class OrdersController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/Orders1
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.PaymentMethod).Include(o => o.Promotion).Include(o => o.Shipping);
            return View(orders.ToList());
        }

        // GET: NhanVienBanHang/Orders1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: NhanVienBanHang/Orders1/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName");
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "FullName");
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName");
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName");
            ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingAddress");
            return View();
        }

        // POST: NhanVienBanHang/Orders1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderNumber,CustomerID,OrderDate,Status,SubTotal,DiscountAmount,TotalAmount,PaymentMethodID,PaymentStatus,Notes,PromotionID,ProcessedBy,ProcessedDate,ShippingID")] Order order)
        {
            // Set status mặc định là Processing
            order.Status = "Processing";
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                // Gọi helper phân loại khách hàng
                CustomerHelper.PhanLoaiKhachHang(db, order.CustomerID.Value);
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "FullName", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
            ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingAddress", order.ShippingID);
            return View(order);
        }

        // GET: NhanVienBanHang/Orders1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "FullName", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
            ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingAddress", order.ShippingID);
            return View(order);
        }

        // POST: NhanVienBanHang/Orders1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderNumber,CustomerID,OrderDate,Status,SubTotal,DiscountAmount,TotalAmount,PaymentMethodID,PaymentStatus,Notes,PromotionID,ProcessedBy,ProcessedDate,ShippingID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "FullName", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
            ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingAddress", order.ShippingID);
            return View(order);
        }

        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }



        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var order = db.Orders.Find(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Sử dụng giá trị đúng theo constraint
            order.Status = "Cancelled"; // Hoặc "Đã hủy" nếu database hỗ trợ Unicode
            order.ProcessedDate = DateTime.Now;

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            catch (Exception ex)
            {
                // Ghi log lỗi
                ModelState.AddModelError("", "Lỗi khi cập nhật trạng thái: " + ex.Message);
                return View(order);
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
