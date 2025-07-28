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
    public class AccountsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: Admin/Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Role).Include(a => a.statusAccount).Include(a => a.statusAccount1);
            return View(accounts.ToList());
        }

        // GET: Admin/Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            ViewBag.IDRole = new SelectList(db.Roles, "IDRole", "TenRole");
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName");
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTK,Email,PasswordHash,UserName,IDRole,statusAccountID")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDRole = new SelectList(db.Roles, "IDRole", "TenRole", account.IDRole);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDRole = new SelectList(db.Roles, "IDRole", "TenRole", account.IDRole);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTK,Email,PasswordHash,UserName,IDRole,statusAccountID")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDRole = new SelectList(db.Roles, "IDRole", "TenRole", account.IDRole);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            ViewBag.statusAccountID = new SelectList(db.statusAccounts, "statusAccountID", "statusName", account.statusAccountID);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
