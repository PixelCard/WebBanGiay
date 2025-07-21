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
    public class ProductComboDetailsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/ProductComboDetails
        public ActionResult Index()
        {
            var productComboDetails = db.ProductComboDetails.Include(p => p.ProductCombo).Include(p => p.Product).Include(p => p.ProductVariant);
            return View(productComboDetails.ToList());
        }

        // GET: NhanVienBanHang/ProductComboDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductComboDetail productComboDetail = db.ProductComboDetails.Find(id);
            if (productComboDetail == null)
            {
                return HttpNotFound();
            }
            return View(productComboDetail);
        }

        // GET: NhanVienBanHang/ProductComboDetails/Create
        public ActionResult Create()
        {
            ViewBag.ComboID = new SelectList(db.ProductComboes, "ComboID", "ComboName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.VariantID = new SelectList(db.ProductVariants, "VariantID", "SKU");
            return View();
        }

        // POST: NhanVienBanHang/ProductComboDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComboDetailID,ComboID,ProductID,VariantID,Quantity")] ProductComboDetail productComboDetail)
        {
            if (ModelState.IsValid)
            {
                db.ProductComboDetails.Add(productComboDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ComboID = new SelectList(db.ProductComboes, "ComboID", "ComboName", productComboDetail.ComboID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productComboDetail.ProductID);
            ViewBag.VariantID = new SelectList(db.ProductVariants, "VariantID", "SKU", productComboDetail.VariantID);
            return View(productComboDetail);
        }

        // GET: NhanVienBanHang/ProductComboDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductComboDetail productComboDetail = db.ProductComboDetails.Find(id);
            if (productComboDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComboID = new SelectList(db.ProductComboes, "ComboID", "ComboName", productComboDetail.ComboID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productComboDetail.ProductID);
            ViewBag.VariantID = new SelectList(db.ProductVariants, "VariantID", "SKU", productComboDetail.VariantID);
            return View(productComboDetail);
        }

        // POST: NhanVienBanHang/ProductComboDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComboDetailID,ComboID,ProductID,VariantID,Quantity")] ProductComboDetail productComboDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productComboDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ComboID = new SelectList(db.ProductComboes, "ComboID", "ComboName", productComboDetail.ComboID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productComboDetail.ProductID);
            ViewBag.VariantID = new SelectList(db.ProductVariants, "VariantID", "SKU", productComboDetail.VariantID);
            return View(productComboDetail);
        }

        // GET: NhanVienBanHang/ProductComboDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductComboDetail productComboDetail = db.ProductComboDetails.Find(id);
            if (productComboDetail == null)
            {
                return HttpNotFound();
            }
            return View(productComboDetail);
        }

        // POST: NhanVienBanHang/ProductComboDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductComboDetail productComboDetail = db.ProductComboDetails.Find(id);
            db.ProductComboDetails.Remove(productComboDetail);
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
