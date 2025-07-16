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
            // Lấy danh sách sản phẩm với variants và sizes
            var productsWithVariants = db.Products
                .Where(p => p.IsActive == true)
                .Select(p => new
                {
                    Product = p,
                    Variants = p.ProductVariants
                        .Where(v => v.IsActive == true)
                        .Select(v => new
                        {
                            Variant = v,
                            Color = v.Color,
                            Size = v.Size,
                            Price = v.Price,
                            StockQuantity = v.StockQuantity
                        }).ToList()
                }).ToList();

            ViewBag.ProductsWithVariants = productsWithVariants;

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