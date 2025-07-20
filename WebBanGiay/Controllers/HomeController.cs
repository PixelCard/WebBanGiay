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

        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm nổi bật (có thể thêm logic filter sau)
            var featuredProducts = _dbContext.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Take(8)
                .ToList();

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

            // Lấy danh sách danh mục
            var categories = _dbContext.Categories
                .Where(c => c.IsActive == true)
                .Take(5)
                .ToList();

            // Lấy danh sách thương hiệu
            var brands = _dbContext.Brands
                .Where(b => b.IsActive == true)
                .Take(5)
                .ToList();

            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.LatestProducts = latestProducts;
            ViewBag.TopRatedProducts = topRatedProducts;
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            return View();
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
    }
}