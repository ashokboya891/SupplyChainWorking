using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Models;
using System;
using System.Reflection.Emit;

namespace SupplyChain.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<RestockRequest> RestockRequests { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        public DbSet<CreateRequest> Requests { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<ApprovalComment> ApprovalComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Optional Fluent Configurations (if needed)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<InventoryLog>()
                .HasOne(log => log.Product)
                .WithMany(p => p.InventoryLogs)
                .HasForeignKey(log => log.ProductId);

            modelBuilder.Entity<InventoryLog>()
                .HasOne(log => log.User)
                .WithMany(u => u.InventoryLogs)
                .HasForeignKey(log => log.UserId);

            modelBuilder.Entity<RestockRequest>()
                .HasOne(r => r.Product)
                .WithMany(p => p.RestockRequests)
                .HasForeignKey(r => r.ProductId);

            modelBuilder.Entity<RestockRequest>()
                .HasOne(r => r.Admin)
                .WithMany(u => u.RestockRequests)
                .HasForeignKey(r => r.AdminId);

            
                modelBuilder.Entity<CreateRequest>()
                    .HasMany(r => r.Approvals)
                    .WithOne(a => a.Request)
                    .HasForeignKey(a => a.RequestId);

                modelBuilder.Entity<CreateRequest>().HasMany(r => r.Comments)
                    .WithOne()
                    .HasForeignKey(c => c.RequestId);
            

        }
    }
}
