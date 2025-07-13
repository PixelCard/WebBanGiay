using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebBanGiay.Models
{
    public partial class CNPM_LT_DBContext : DbContext
    {
        public CNPM_LT_DBContext()
            : base("name=CNPM_LT_DBContext")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PriceHistory> PriceHistories { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductVariant> ProductVariants { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasMany(e => e.Employees)
                .WithOptional(e => e.Admin)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.InventoryTransactions)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.ProcessedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PriceHistories)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.ChangedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ProductReviews)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.ApprovedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Promotions)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.CreatedBy);
        }
    }
}
