using System;

namespace WebBanGiay.Models
{
    public class CartItem
    {
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
} 