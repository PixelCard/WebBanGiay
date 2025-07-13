using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanGiay.Models.model_class
{
    public class order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // "Pending", "Processing", "Completed", "Cancelled"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [ForeignKey("PaymentMethod")]
        public int PaymentMethodID { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } // "Pending", "Paid", "Failed", "Refunded"

        [Required]
        [StringLength(255)]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(20)]
        public string ShippingPhone { get; set; }

        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [ForeignKey("Promotion")]
        public int? PromotionID { get; set; }

        [ForeignKey("Employee")]
        public int? ProcessedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ProcessedDate { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}