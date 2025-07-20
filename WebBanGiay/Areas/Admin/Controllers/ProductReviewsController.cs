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

namespace WebBanGiay.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductReviewsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: Admin/ProductReviews
        public ActionResult Index()
        {
            var productReviews = db.ProductReviews.Include(p => p.Customer).Include(p => p.Employee).Include(p => p.Order).Include(p => p.Product);
            return View(productReviews.ToList());
        }

        // GET: Admin/ProductReviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        // GET: Admin/ProductReviews/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username");
            ViewBag.ApprovedBy = new SelectList(db.Employees, "EmployeeID", "Username");
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Admin/ProductReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReviewID,ProductID,CustomerID,OrderID,Rating,Title,Comment,IsVerifiedPurchase,IsApproved,ReviewDate,ApprovedBy,ApprovedDate")] ProductReview productReview)
        {
            if (ModelState.IsValid)
            {
                db.ProductReviews.Add(productReview);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", productReview.CustomerID);
            ViewBag.ApprovedBy = new SelectList(db.Employees, "EmployeeID", "Username", productReview.ApprovedBy);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", productReview.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // GET: Admin/ProductReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", productReview.CustomerID);
            ViewBag.ApprovedBy = new SelectList(db.Employees, "EmployeeID", "Username", productReview.ApprovedBy);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", productReview.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // POST: Admin/ProductReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewID,ProductID,CustomerID,OrderID,Rating,Title,Comment,IsVerifiedPurchase,IsApproved,ReviewDate,ApprovedBy,ApprovedDate")] ProductReview productReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Username", productReview.CustomerID);
            ViewBag.ApprovedBy = new SelectList(db.Employees, "EmployeeID", "Username", productReview.ApprovedBy);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNumber", productReview.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // GET: Admin/ProductReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        // POST: Admin/ProductReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductReview productReview = db.ProductReviews.Find(id);
            db.ProductReviews.Remove(productReview);
            db.SaveChanges();
            return RedirectToAction("Index");
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
