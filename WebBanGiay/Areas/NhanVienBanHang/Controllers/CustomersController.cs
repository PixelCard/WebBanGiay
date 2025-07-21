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
    public class CustomersController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Account).Include(c => c.LoaiKhachHang);
            return View(customers.ToList());
        }

        // GET: NhanVienBanHang/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: NhanVienBanHang/Customers/Create
        public ActionResult Create()
        {
            ViewBag.IDAccount = new SelectList(db.Accounts, "IDTK", "Email");
            ViewBag.LoaiID = new SelectList(db.LoaiKhachHangs, "LoaiID", "LoaiKhachHangName");
            return View();
        }

        // POST: NhanVienBanHang/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,FullName,Phone,Address,DateOfBirth,Gender,PreferredSize,PreferredBrand,Newsletter,SMSNotification,IsActive,CreatedDate,LastLogin,IDAccount,LoaiID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.LoaiID = 1;
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDAccount = new SelectList(db.Accounts, "IDTK", "Email", customer.IDAccount);
            ViewBag.LoaiID = new SelectList(db.LoaiKhachHangs, "LoaiID", "LoaiKhachHangName", customer.LoaiID);
            return View(customer);
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
