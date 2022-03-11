using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Paint_Ecommerce.Models
{
    public class Db:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Verify> Verifies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<PickPoint> PickPoints { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        public System.Data.Entity.DbSet<Paint_Ecommerce.Models.UserProfileVM> UserProfileVMs { get; set; }

        public System.Data.Entity.DbSet<Paint_Ecommerce.Models.Booking> Bookings { get; set; }

        public System.Data.Entity.DbSet<Paint_Ecommerce.Models.BookingVM> BookingVMs { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }

        public System.Data.Entity.DbSet<Paint_Ecommerce.Models.RefundVM> RefundVMs { get; set; }
        //public DbSet<Delivery> Deliveries { get; set; }
        //public DbSet<QrCode> QrCodes { get; set; }
        //public DbSet<Qcode> Qcodes { get; set; }
        //public DbSet<Scan> Scans { get; set; }
    }
}