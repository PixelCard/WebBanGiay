using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    public class HomeController : Controller
    {
        private CNPM_LTEntities _dbContext = new CNPM_LTEntities();

        public ActionResult Index(int? categoryId = null)
        {
            // Lấy danh sách sản phẩm nổi bật (có thể thêm logic filter sau)
            var featuredProductsQuery = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants);

            // Filter theo category nếu có
            if (categoryId.HasValue)
            {
                featuredProductsQuery = featuredProductsQuery.Where(p => p.CategoryID == categoryId.Value);
            }

            var featuredProducts = featuredProductsQuery.Take(8).ToList();

            // Lấy danh sách sản phẩm mới nhất
            var latestProducts = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .OrderByDescending(p => p.CreatedDate)
                .Take(6)
                .ToList();

            // Lấy danh sách sản phẩm được đánh giá cao
            var topRatedProducts = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductReviews)
                .Where(p => p.ProductReviews.Any(r => r.IsApproved == true))
                .OrderByDescending(p => p.ProductReviews.Where(r => r.IsApproved == true).Average(r => r.Rating))
                .Take(6)
                .ToList();

            // Nếu không có sản phẩm nào có review, lấy sản phẩm mới nhất
            if (!topRatedProducts.Any())
            {
                topRatedProducts = _dbContext.Products
                    .Where(p => p.IsActive == true)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .Include(p => p.ProductReviews)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(6)
                    .ToList();
            }

            // Lấy danh sách danh mục có sản phẩm nổi bật
            var categoriesWithProducts = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Select(p => p.Category)
                .Where(c => c != null && c.IsActive == true)
                .Distinct()
                .ToList();

            // Lấy danh sách thương hiệu
            var brands = _dbContext.Brands
                .Where(b => b.IsActive == true)
                .Take(5)
                .ToList();

            // Lấy danh sách combo sản phẩm đang hoạt động
            var productCombos = _dbContext.ProductComboes
                .Where(c => c.IsActive == true)
                .OrderByDescending(c => c.startDate)
                .ToList();

            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.LatestProducts = latestProducts;
            ViewBag.TopRatedProducts = topRatedProducts;
            ViewBag.Categories = categoriesWithProducts;
            ViewBag.Brands = brands;
            ViewBag.ProductCombos = productCombos;
            ViewBag.SelectedCategory = categoryId;

            return View();
        }

        public ActionResult Search(string q = "", int? categoryId = null, int? brandId = null, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = "name", int page = 1)
        {
            var searchQuery = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(q))
            {
                searchQuery = searchQuery.Where(p => 
                    (p.ProductName != null && p.ProductName.Contains(q)) || 
                    (p.Description != null && p.Description.Contains(q)) ||
                    (p.Category != null && p.Category.CategoryName != null && p.Category.CategoryName.Contains(q)) ||
                    (p.Brand != null && p.Brand.BrandName != null && p.Brand.BrandName.Contains(q))
                );
            }

            // Filter theo category
            if (categoryId.HasValue)
            {
                searchQuery = searchQuery.Where(p => p.CategoryID == categoryId.Value);
            }

            // Filter theo brand
            if (brandId.HasValue)
            {
                searchQuery = searchQuery.Where(p => p.BrandID == brandId.Value);
            }

            // Filter theo giá
            if (minPrice.HasValue)
            {
                searchQuery = searchQuery.Where(p => p.ProductVariants.Any(v => v.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                searchQuery = searchQuery.Where(p => p.ProductVariants.Any(v => v.Price <= maxPrice.Value));
            }

            // Sắp xếp
            switch (sortBy.ToLower())
            {
                case "price_asc":
                    searchQuery = searchQuery.OrderBy(p => p.ProductVariants.Min(v => v.Price));
                    break;
                case "price_desc":
                    searchQuery = searchQuery.OrderByDescending(p => p.ProductVariants.Max(v => v.Price));
                    break;
                case "name":
                default:
                    searchQuery = searchQuery.OrderBy(p => p.ProductName);
                    break;
            }

            // Phân trang
            int pageSize = 12;
            int totalItems = searchQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            var products = searchQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách categories và brands cho filter
            var categories = _dbContext.Categories
                .Where(c => c.IsActive == true)
                .ToList();

            var brands = _dbContext.Brands
                .Where(b => b.IsActive == true)
                .ToList();

            ViewBag.SearchQuery = q;
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedBrand = brandId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Products = products;

            return View();
        }

        private string NormalizeForCssClass(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "default";

            return text.ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("Đ", "d")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ả", "a")
                .Replace("ã", "a")
                .Replace("ạ", "a")
                .Replace("ă", "a")
                .Replace("ắ", "a")
                .Replace("ằ", "a")
                .Replace("ẳ", "a")
                .Replace("ẵ", "a")
                .Replace("ặ", "a")
                .Replace("â", "a")
                .Replace("ấ", "a")
                .Replace("ầ", "a")
                .Replace("ẩ", "a")
                .Replace("ẫ", "a")
                .Replace("ậ", "a")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ẻ", "e")
                .Replace("ẽ", "e")
                .Replace("ẹ", "e")
                .Replace("ê", "e")
                .Replace("ế", "e")
                .Replace("ề", "e")
                .Replace("ể", "e")
                .Replace("ễ", "e")
                .Replace("ệ", "e")
                .Replace("í", "i")
                .Replace("ì", "i")
                .Replace("ỉ", "i")
                .Replace("ĩ", "i")
                .Replace("ị", "i")
                .Replace("ó", "o")
                .Replace("ò", "o")
                .Replace("ỏ", "o")
                .Replace("õ", "o")
                .Replace("ọ", "o")
                .Replace("ô", "o")
                .Replace("ố", "o")
                .Replace("ồ", "o")
                .Replace("ổ", "o")
                .Replace("ỗ", "o")
                .Replace("ộ", "o")
                .Replace("ơ", "o")
                .Replace("ớ", "o")
                .Replace("ờ", "o")
                .Replace("ở", "o")
                .Replace("ỡ", "o")
                .Replace("ợ", "o")
                .Replace("ú", "u")
                .Replace("ù", "u")
                .Replace("ủ", "u")
                .Replace("ũ", "u")
                .Replace("ụ", "u")
                .Replace("ư", "u")
                .Replace("ứ", "u")
                .Replace("ừ", "u")
                .Replace("ử", "u")
                .Replace("ữ", "u")
                .Replace("ự", "u")
                .Replace("ý", "y")
                .Replace("ỳ", "y")
                .Replace("ỷ", "y")
                .Replace("ỹ", "y")
                .Replace("ỵ", "y");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public JsonResult GetComboProducts(int comboId)
        {
            try
            {
                var comboDetails = _dbContext.ProductComboDetails
                    .Where(cd => cd.ComboID == comboId)
                    .Include(cd => cd.Product)
                    .Include(cd => cd.ProductVariant)
                    .Include(cd => cd.ProductVariant.Color)
                    .Include(cd => cd.ProductVariant.Size)
                    .ToList();


                var products = comboDetails.Select(cd => new
                {
                    productName = cd.Product?.ProductName ?? "Sản phẩm không xác định",
                    colorName = cd.ProductVariant?.Color?.ColorName ?? "Không xác định",
                    sizeName = cd.ProductVariant?.Size?.SizeName ?? "Không xác định",
                    quantity = cd.Quantity,
                    price = cd.ProductVariant?.Price ?? 0,
                    imageUrl = cd.Product?.GetFirstImageUrl() ?? ""
                }).ToList();

                return Json(new
                {
                    success = true,
                    products = products
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tải thông tin sản phẩm: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}