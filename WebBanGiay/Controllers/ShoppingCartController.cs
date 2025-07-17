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


        [HttpPost]
        public JsonResult UpdateQuantity(int variantId, int quantity)
        {
            // Lấy cart từ session
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem>;
            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            // Tìm sản phẩm theo variantId
            var item = cart.FirstOrDefault(x => x.VariantID == variantId);
            if (item == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Cập nhật số lượng và tổng tiền
            item.Quantity = quantity;

            // Lưu lại cart vào session
            Session["Cart"] = cart;

            // Tính lại tổng tiền giỏ hàng
            var cartTotal = cart.Sum(x => x.TotalPrice);

            return Json(new
            {
                success = true,
                totalPrice = item.TotalPrice,
                cartTotal = cartTotal,
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        [HttpPost]
        public JsonResult RemoveItem(int variantId)
        {
            // Lấy cart từ session
            var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem>;
            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            // Tìm và xóa sản phẩm theo variantId
            var item = cart.FirstOrDefault(x => x.VariantID == variantId);
            if (item == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            cart.Remove(item);
            Session["Cart"] = cart;

            var cartTotal = cart.Sum(x => x.TotalPrice);
            var cartCount = cart.Sum(x => x.Quantity);

            return Json(new
            {
                success = true,
                cartTotal = cartTotal,
                cartCount = cartCount
            });
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