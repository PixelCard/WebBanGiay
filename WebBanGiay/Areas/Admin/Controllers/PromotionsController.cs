using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    public class PromotionsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: Admin/Promotions
        public ActionResult Index()
        {
            var promotions = db.Promotions.Include(p => p.Employee);
            return View(promotions.ToList());
        }

        // GET: Admin/Promotions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // GET: Admin/Promotions/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Employees, "EmployeeID", "FullName");
            return View();
        }

        // POST: Admin/Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PromotionID,PromotionName,Description,DiscountType,DiscountValue,MinOrderAmount,MaxDiscountAmount,StartDate,EndDate,IsActive,UsageLimit,UsedCount,CreatedBy,CreatedDate")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                db.Promotions.Add(promotion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Employees, "EmployeeID", "FullName", promotion.CreatedBy);
            return View(promotion);
        }

        // GET: Admin/Promotions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Employees, "EmployeeID", "FullName", promotion.CreatedBy);
            return View(promotion);
        }

        // POST: Admin/Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PromotionID,PromotionName,Description,DiscountType,DiscountValue,MinOrderAmount,MaxDiscountAmount,StartDate,EndDate,IsActive,UsageLimit,UsedCount,CreatedBy,CreatedDate")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promotion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Employees, "EmployeeID", "FullName", promotion.CreatedBy);
            return View(promotion);
        }

        // GET: Admin/Promotions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Admin/Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Promotion promotion = db.Promotions.Find(id);
            db.Promotions.Remove(promotion);
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
