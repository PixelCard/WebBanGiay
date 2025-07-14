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
    public class ProductVariantsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: Admin/ProductVariants
        public ActionResult Index()
        {
            var productVariants = db.ProductVariants.Include(p => p.Color).Include(p => p.Product).Include(p => p.Size);
            return View(productVariants.ToList());
        }

        // GET: Admin/ProductVariants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            return View(productVariant);
        }

        // GET: Admin/ProductVariants/Create
        public ActionResult Create()
        {
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "ColorName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeName");
            return View();
        }

        // POST: Admin/ProductVariants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VariantID,ProductID,ColorID,SizeID,SKU,Price,StockQuantity,IsActive")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.ProductVariants.Add(productVariant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "ColorName", productVariant.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productVariant.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeName", productVariant.SizeID);
            return View(productVariant);
        }

        // GET: Admin/ProductVariants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "ColorName", productVariant.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productVariant.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeName", productVariant.SizeID);
            return View(productVariant);
        }

        // POST: Admin/ProductVariants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VariantID,ProductID,ColorID,SizeID,SKU,Price,StockQuantity,IsActive")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productVariant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "ColorName", productVariant.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productVariant.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeName", productVariant.SizeID);
            return View(productVariant);
        }

        // GET: Admin/ProductVariants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            return View(productVariant);
        }

        // POST: Admin/ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductVariant productVariant = db.ProductVariants.Find(id);
            db.ProductVariants.Remove(productVariant);
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
