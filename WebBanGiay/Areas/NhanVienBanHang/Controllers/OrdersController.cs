using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.NhanVienBanHang.Controllers
{
    public class OrdersController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.PaymentMethod).Include(o => o.Promotion);
            return View(orders.ToList());
        }

        // GET: NhanVienBanHang/Orders/Details/5
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

        // GET: NhanVienBanHang/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "Username");
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName");
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName");
            return View();
        }

        // POST: NhanVienBanHang/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderNumber,CustomerID,OrderDate,Status,SubTotal,DiscountAmount,ShippingFee,TotalAmount,PaymentMethodID,PaymentStatus,ShippingAddress,ShippingPhone,ShippingName,Notes,PromotionID,ProcessedBy,ProcessedDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "Username", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
            return View(order);
        }

        // GET: NhanVienBanHang/Orders/Edit/5
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
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "Username", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
            return View(order);
        }

        // POST: NhanVienBanHang/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderNumber,CustomerID,OrderDate,Status,SubTotal,DiscountAmount,ShippingFee,TotalAmount,PaymentMethodID,PaymentStatus,ShippingAddress,ShippingPhone,ShippingName,Notes,PromotionID,ProcessedBy,ProcessedDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", order.CustomerID);
            ViewBag.ProcessedBy = new SelectList(db.Employees, "EmployeeID", "Username", order.ProcessedBy);
            ViewBag.PaymentMethodID = new SelectList(db.PaymentMethods, "PaymentMethodID", "MethodName", order.PaymentMethodID);
            ViewBag.PromotionID = new SelectList(db.Promotions, "PromotionID", "PromotionName", order.PromotionID);
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
