using System;
using System.Linq;
using System.Web.Mvc;
using WebBanGiay.Filters;
using WebBanGiay.Models;
using WebBanGiay.Models.model_class;


namespace WebBanGiay.Areas.NhanVienBanHang.Controllers
{
    [NhanVienBanHangAuthorize]
    public class StatisticsController : Controller
    {
        private CNPM_LTEntities db = new CNPM_LTEntities();

        // GET: NhanVienBanHang/Statistics/OrdersByDate
        public ActionResult OrdersByDate(DateTime? from, DateTime? to)
        {
            var query = db.Orders.AsQueryable();

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);

            // Lấy dữ liệu về trước, sau đó group by ngày trong bộ nhớ
            var orders = query.Where(o => o.OrderDate.HasValue).ToList();

            var result = orders
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new OrderByDate
                {
                    Date = g.Key,
                    TotalOrders = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.From = from;
            ViewBag.To = to;
            return View(result);
        }
    }
}