using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    public class CheckOutController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: CheckOut
        public ActionResult Checkoutpage()
        {
            // Check if user is logged in
            if (Session["UserID"] != null)
            {
                int userID = (int)Session["UserID"];
                
                // Get customer information
                var customer = db.Customers
                    .Include("Account")
                    .Where(c => c.IDAccount == userID && c.IsActive == true)
                    .FirstOrDefault();

                if (customer != null)
                {
                    ViewBag.Customer = customer;
                    ViewBag.IsLoggedIn = true;
                }
                else
                {
                    ViewBag.IsLoggedIn = false;
                }
            }
            else
            {
                ViewBag.IsLoggedIn = false;
            }

            // Get cart information
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem>;
            if (cart != null && cart.Any())
            {
                ViewBag.Cart = cart;
                ViewBag.CartTotal = cart.Sum(item => item.TotalPrice);
            }

            else
            {
                // Redirect to shopping cart if cart is empty
                return RedirectToAction("ShoppingCartPage", "ShoppingCart");
            }

            return View();
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