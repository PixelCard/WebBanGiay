using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;
using System.Data.Entity; // Added for .Include()

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

        [HttpPost]
        public JsonResult AddComboToCart(int comboId)
        {
            try
            {
                // Kiểm tra combo có tồn tại và đang hoạt động không
                var combo = db.ProductComboes.FirstOrDefault(c => c.ComboID == comboId && c.IsActive == true);
                if (combo == null)
                {
                    return Json(new { success = false, message = "Combo không tồn tại hoặc đã bị vô hiệu hóa." });
                }

                // Kiểm tra combo có hết hạn chưa
                if (combo.endDate.HasValue && combo.endDate.Value < DateTime.Now)
                {
                    return Json(new { success = false, message = "Combo đã hết hạn." });
                }

                // Lấy danh sách sản phẩm trong combo
                var comboDetails = db.ProductComboDetails
                    .Where(cd => cd.ComboID == comboId)
                    .Include(cd => cd.Product)
                    .Include(cd => cd.ProductVariant)
                    .Include(cd => cd.ProductVariant.Color)
                    .Include(cd => cd.ProductVariant.Size)
                    .ToList();

                if (!comboDetails.Any())
                {
                    return Json(new { success = false, message = "Combo không có sản phẩm nào." });
                }

                // Kiểm tra tất cả sản phẩm trong combo có còn hàng không
                foreach (var detail in comboDetails)
                {
                    if (detail.ProductVariant == null || detail.ProductVariant.StockQuantity < detail.Quantity)
                    {
                        return Json(new { success = false, message = $"Sản phẩm {detail.Product?.ProductName} không đủ hàng trong combo." });
                    }
                }

                // Lấy cart từ session
                var cart = Session["Cart"] as List<WebBanGiay.Models.model_class.CartItem> ?? new List<WebBanGiay.Models.model_class.CartItem>();

                // Thêm từng sản phẩm trong combo vào giỏ hàng
                foreach (var detail in comboDetails)
                {
                    // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                    var existingItem = cart.FirstOrDefault(x => x.VariantID == detail.VariantID);
                    
                    if (existingItem != null)
                    {
                        // Cập nhật số lượng nếu đã có
                        existingItem.Quantity += detail.Quantity;
                    }
                    else
                    {
                        // Thêm mới nếu chưa có
                        var cartItem = new WebBanGiay.Models.model_class.CartItem
                        {
                            ProductID = detail?.ProductID ?? 0,
                            VariantID = detail?.VariantID ?? 0,
                            ProductName = detail.Product?.ProductName ?? "Sản phẩm không xác định",
                            ColorID = detail.ProductVariant?.ColorID ?? 0,
                            ColorName = detail.ProductVariant?.Color?.ColorName ?? "Không xác định",
                            ColorCode = detail.ProductVariant?.Color?.ColorCode ?? "#000000",
                            SizeID = detail.ProductVariant?.SizeID ?? 0,
                            SizeName = detail.ProductVariant?.Size?.SizeName ?? "Không xác định",
                            Price = detail.ProductVariant?.Price ?? 0,
                            Quantity = detail?.Quantity ?? 0,
                            ProductImage = detail.Product?.GetFirstImageUrl() ?? ""
                        };
                        cart.Add(cartItem);
                    }
                }

                // Lưu cart vào session
                Session["Cart"] = cart;

                // Tính tổng tiền và số lượng
                var cartTotal = cart.Sum(x => x.TotalPrice);
                var cartCount = cart.Sum(x => x.Quantity);

                return Json(new
                {
                    success = true,
                    message = $"Đã thêm combo '{combo.ComboName}' vào giỏ hàng!",
                    cartTotal = cartTotal.ToString("N0"),
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm combo vào giỏ hàng: " + ex.Message });
            }
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