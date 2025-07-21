using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanGiay.Models;

namespace WebBanGiay.helper
{
    public static class CustomerHelper
    {
        public static void PhanLoaiKhachHang(CNPM_LTEntities db, int customerId)
        {
            var customer = db.Customers.Find(customerId);
            if (customer == null) return;

            int soDon = db.Orders.Count(o => o.CustomerID == customerId);

            if (soDon >= 30)
                customer.LoaiID = 4; // CEO
            else if (soDon >= 10)
                customer.LoaiID = 3; // VIP
            else if (soDon >= 5)
                customer.LoaiID = 2; // Thân thiết
            else
                customer.LoaiID = 1; // Thường

            db.SaveChanges();
        }
    }
}