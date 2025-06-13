using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Enum;
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

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryApprovalStage> CategoryApprovalStages { get; set; }

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

            modelBuilder.Entity<Category>().HasData(
                   new Category { Id = 1, Name = "Hardware" },
                       new Category { Id = 2, Name = "Software" },
                     new Category { Id = 3, Name = "Miscellaneous" }
                );

            modelBuilder.Entity<CategoryApprovalStage>().HasData(
                new CategoryApprovalStage { Id = 1, CategoryId = 1, Role = ApprovalRole.Manager, StageOrder = 1 },
                new CategoryApprovalStage { Id = 2, CategoryId = 1, Role = ApprovalRole.IT, StageOrder = 2 },

                new CategoryApprovalStage { Id = 3, CategoryId = 2, Role = ApprovalRole.Finance, StageOrder = 1 },
                new CategoryApprovalStage { Id = 4, CategoryId = 2, Role = ApprovalRole.IT, StageOrder = 2 },

                new CategoryApprovalStage { Id = 5, CategoryId = 3, Role = ApprovalRole.Manager, StageOrder = 1 }
            );



        }
    }
}
