using System;

namespace WebBanGiay.Models
{
    public class VariantDTO
    {
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
} 