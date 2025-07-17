using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    public class ShoppingCartController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: ShoppingCart
        public ActionResult ShoppingCartPage()
        {
            // Lấy cart từ session
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem> ?? new List<WebBanGiay.Models.model_class.CartItem>();
            
            ViewBag.Cart = cart;
            ViewBag.CartTotal = cart.Sum(item => item.TotalPrice);
            
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