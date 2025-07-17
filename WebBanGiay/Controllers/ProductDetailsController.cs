using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;
using WebBanGiay.Models.model_class;

namespace WebBanGiay.Controllers
{
    public class ProductDetailsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: ProductDetails
        public ActionResult ProductDetailsPage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // Disable proxy creation to avoid proxy object issues
            db.Configuration.ProxyCreationEnabled = false;

            // Fetch product with all related data
            var product = db.Products
                .Include("Brand")
                .Include("Category")
                .Include("Material")
                .Include("ProductImages")
                .Include("ProductVariants")
                .Include("ProductVariants.Color")
                .Include("ProductVariants.Size")
                .Include("ProductReviews")
                .Include("ProductReviews.Customer")
                .Where(p => p.ProductID == id && p.IsActive == true)
                .FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            // Ensure related data is loaded properly
            if (product.Brand != null)
            {
                db.Entry(product).Reference(p => p.Brand).Load();
            }
            if (product.Category != null)
            {
                db.Entry(product).Reference(p => p.Category).Load();
            }
            if (product.Material != null)
            {
                db.Entry(product).Reference(p => p.Material).Load();
            }

            // Get available sizes and colors for this product
            var variants = product.ProductVariants.Where(v => v.IsActive == true && v.StockQuantity > 0).ToList();
            var availableSizes = variants.Select(v => v.Size).Distinct().ToList();
            var availableColors = variants.Select(v => v.Color).Distinct().ToList();

            // Tạo DTO để tránh circular reference
            var variantDTOs = variants.Select(v => new VariantDTO
            {
                VariantID = v.VariantID,
                ProductID = v.ProductID ?? 0,
                ColorID = v.ColorID ?? 0,
                ColorName = v.Color?.ColorName ?? "",
                ColorCode = v.Color?.ColorCode ?? "",
                SizeID = v.SizeID ?? 0,
                SizeName = v.Size?.SizeName ?? "",
                Price = v.Price,
                StockQuantity = v.StockQuantity ?? 0,
                IsActive = v.IsActive ?? false
            }).ToList();

            ViewBag.AvailableSizes = availableSizes;
            ViewBag.AvailableColors = availableColors;
            ViewBag.Variants = variantDTOs; // Sử dụng DTO thay vì entity

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult AddToCart(int variantId, int quantity)
        {
            try
            {
                // Lấy thông tin variant
                var variant = db.ProductVariants
                    .Include("Product")
                    .Include("Product.ProductImages")
                    .Include("Color")
                    .Include("Size")
                    .Where(v => v.VariantID == variantId && v.IsActive == true)
                    .FirstOrDefault();

                if (variant == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                if (variant.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Số lượng trong kho không đủ!" });
                }

                // Tạo cart item
                var cartItem = new CartItem
                {
                    VariantID = variant.VariantID,
                    ProductID = variant.ProductID ?? 0,
                    ProductName = variant.Product?.ProductName ?? "",
                    ProductImage = variant.Product?.ProductImages?.FirstOrDefault()?.ImageURL ?? "~/Content/img/product/default-product.jpg",
                    ColorID = variant.ColorID ?? 0,
                    ColorName = variant.Color?.ColorName ?? "",
                    ColorCode = variant.Color?.ColorCode ?? "",
                    SizeID = variant.SizeID ?? 0,
                    SizeName = variant.Size?.SizeName ?? "",
                    Price = variant.Price,
                    Quantity = quantity
                };

                // Lấy cart từ session
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

                // Kiểm tra xem sản phẩm đã có trong cart chưa
                var existingItem = cart.FirstOrDefault(item => item.VariantID == variantId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(cartItem);
                }

                // Lưu cart vào session
                Session["Cart"] = cart;

                return Json(new { 
                    success = true, 
                    message = "Đã thêm vào giỏ hàng!", 
                    cartCount = cart.Count,
                    cartTotal = cart.Sum(item => item.TotalPrice)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}