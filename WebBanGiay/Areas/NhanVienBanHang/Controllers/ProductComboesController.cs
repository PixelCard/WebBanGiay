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
    public class ProductComboesController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/ProductComboes
        public ActionResult Index()
        {
            return View(db.ProductComboes.ToList());
        }

        // GET: NhanVienBanHang/ProductComboes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCombo productCombo = db.ProductComboes.Find(id);
            if (productCombo == null)
            {
                return HttpNotFound();
            }
            return View(productCombo);
        }

        // GET: NhanVienBanHang/ProductComboes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NhanVienBanHang/ProductComboes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComboID,ComboName,Description,ComboPrice,ImageURL,IsActive,startDate,endDate")] ProductCombo productCombo)
        {
            if (ModelState.IsValid)
            {
                db.ProductComboes.Add(productCombo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productCombo);
        }

        // GET: NhanVienBanHang/ProductComboes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCombo productCombo = db.ProductComboes.Find(id);
            if (productCombo == null)
            {
                return HttpNotFound();
            }
            return View(productCombo);
        }

        // POST: NhanVienBanHang/ProductComboes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComboID,ComboName,Description,ComboPrice,ImageURL,IsActive,startDate,endDate")] ProductCombo productCombo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productCombo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productCombo);
        }

        // GET: NhanVienBanHang/ProductComboes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCombo productCombo = db.ProductComboes.Find(id);
            if (productCombo == null)
            {
                return HttpNotFound();
            }
            return View(productCombo);
        }

        // POST: NhanVienBanHang/ProductComboes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCombo productCombo = db.ProductComboes.Find(id);
            db.ProductComboes.Remove(productCombo);
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
