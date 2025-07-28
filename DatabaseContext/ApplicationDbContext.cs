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

        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
             .ToTable(tb => tb.HasTrigger("trg_CheckStockAfterUpdate"));

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

            modelBuilder.Entity<ProductPhoto>()
                    .HasOne(p => p.Product)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(p => p.ProductId);

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


            modelBuilder.Entity<ProductType>().HasData(
                 new ProductType { Id = 1, Name = "Electronics" },
                 new ProductType { Id = 2, Name = "Furniture" },
                 new ProductType { Id = 3, Name = "Educational" },
                 new ProductType { Id = 4, Name = "Kitchen" }
                );

            modelBuilder.Entity<ProductBrand>().HasData(
                new ProductBrand { Id = 1, Name = "Logitech" },
                new ProductBrand { Id = 2, Name = "IKEA" },
                new ProductBrand { Id = 3, Name = "Oxford" },
                new ProductBrand { Id = 4, Name = "Prestige" },
                new ProductBrand { Id = 5, Name = "HP" }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Wireless Mouse", Description = "Ergonomic Bluetooth wireless mouse", Price = 899, CurrentStock = 50, Threshold = 10, ProductTypeId = 1, ProductBrandId = 1 },
                new Product { ProductId = 2, Name = "Gaming Keyboard", Description = "RGB mechanical keyboard with macro support", Price = 1899, CurrentStock = 30, Threshold = 5, ProductTypeId = 1, ProductBrandId = 1 },
                new Product { ProductId = 3, Name = "Office Chair", Description = "Adjustable mesh back office chair", Price = 4999, CurrentStock = 20, Threshold = 3, ProductTypeId = 2, ProductBrandId = 2 },
                new Product { ProductId = 4, Name = "Study Table", Description = "Minimalist study desk with drawers", Price = 3599, CurrentStock = 25, Threshold = 5, ProductTypeId = 2, ProductBrandId = 2 },
                new Product { ProductId = 5, Name = "Mathematics Textbook", Description = "CBSE Grade 10 Mathematics", Price = 399, CurrentStock = 70, Threshold = 15, ProductTypeId = 3, ProductBrandId = 3 },
                new Product { ProductId = 6, Name = "Science Workbook", Description = "Lab workbook with experiment logs", Price = 299, CurrentStock = 50, Threshold = 10, ProductTypeId = 3, ProductBrandId = 3 },
                new Product { ProductId = 7, Name = "Electric Kettle", Description = "1.5L electric kettle with auto shut-off", Price = 1499, CurrentStock = 40, Threshold = 8, ProductTypeId = 4, ProductBrandId = 4 },
                new Product { ProductId = 8, Name = "Non-stick Frying Pan", Description = "24cm frying pan with glass lid", Price = 799, CurrentStock = 35, Threshold = 6, ProductTypeId = 4, ProductBrandId = 4 },
                new Product { ProductId = 9, Name = "Inkjet Printer", Description = "Wireless printer with duplex printing", Price = 6599, CurrentStock = 10, Threshold = 2, ProductTypeId = 1, ProductBrandId = 5 },
                new Product { ProductId = 10, Name = "Laptop Backpack", Description = "Waterproof 15.6-inch laptop backpack", Price = 999, CurrentStock = 50, Threshold = 7, ProductTypeId = 2, ProductBrandId = 2 },
                new Product { ProductId = 11, Name = "Graph Notebook", Description = "200-page graph notebook for math", Price = 199, CurrentStock = 80, Threshold = 10, ProductTypeId = 3, ProductBrandId = 3 },
                new Product { ProductId = 12, Name = "Hand Blender", Description = "Compact 300W kitchen blender", Price = 1199, CurrentStock = 30, Threshold = 5, ProductTypeId = 4, ProductBrandId = 4 },
                new Product { ProductId = 13, Name = "Wireless Earbuds", Description = "Noise-cancelling TWS with mic", Price = 2499, CurrentStock = 40, Threshold = 8, ProductTypeId = 1, ProductBrandId = 1 },
                new Product { ProductId = 14, Name = "Book Shelf", Description = "5-layer wooden shelf for study room", Price = 2499, CurrentStock = 15, Threshold = 3, ProductTypeId = 2, ProductBrandId = 2 },
                new Product { ProductId = 15, Name = "Physics Guide", Description = "Competitive exam prep book for Physics", Price = 449, CurrentStock = 60, Threshold = 12, ProductTypeId = 3, ProductBrandId = 3 },
                new Product { ProductId = 16, Name = "Steel Pressure Cooker", Description = "5L stainless steel cooker", Price = 2299, CurrentStock = 25, Threshold = 5, ProductTypeId = 4, ProductBrandId = 4 },
                new Product { ProductId = 17, Name = "Bluetooth Speaker", Description = "10W portable waterproof speaker", Price = 1999, CurrentStock = 35, Threshold = 7, ProductTypeId = 1, ProductBrandId = 1 },
                new Product { ProductId = 18, Name = "Dining Table Set", Description = "4-seater modern dining set", Price = 7999, CurrentStock = 8, Threshold = 2, ProductTypeId = 2, ProductBrandId = 2 },
                new Product { ProductId = 19, Name = "English Grammar Book", Description = "Comprehensive English grammar reference", Price = 349, CurrentStock = 55, Threshold = 10, ProductTypeId = 3, ProductBrandId = 3 },
                new Product { ProductId = 20, Name = "Toaster", Description = "2-slice toaster with browning control", Price = 999, CurrentStock = 22, Threshold = 5, ProductTypeId = 4, ProductBrandId = 4 }
            );

            modelBuilder.Entity<ProductPhoto>().HasData(
                 new ProductPhoto
                  {
                     PhotoId = 1,
                     ProductId = 1,
                     Url = "https://res.cloudinary.com/your-cloud/image/upload/v162876/wireless_mouse.jpg",
                     PublicId = "wireless_mouse",
                     IsPrimary = true
                 },
                 new ProductPhoto
                        {
                            PhotoId = 2,
                            ProductId = 7,
                            Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/bluetooth-speaker_egazpg.webp",
                            PublicId = "bluetooth-speaker_egazpg",
                            IsPrimary = true
                  },
                  new ProductPhoto
                      {
                          PhotoId = 3,
                          ProductId = 2,
                          Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/keyboard_qisjqv.webp",
                          PublicId = "keyboard_qisjqv",
                          IsPrimary = true
                      },
                   new ProductPhoto
                    {
                                        PhotoId = 4,
                                        ProductId = 20,
                                        Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/toaster_ujtplk.webp",
                                        PublicId = "toaster_ujtplk",
                                        IsPrimary = true
                    },
                    new ProductPhoto
                    {
                                PhotoId = 5,
                                ProductId = 19,
                                Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/english_siabfk.webp",
                                PublicId = "english_siabfk",
                                IsPrimary = true
                    },
                    new ProductPhoto
                    {
                                PhotoId = 6,
                                ProductId = 16,
                                Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/kettle_bbamzn.webp",
                                PublicId = "kettle_bbamzn",
                                IsPrimary = true
                                
                    },
                  new ProductPhoto
                  {
                                 PhotoId = 7,
                                 ProductId = 19,
                                 Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422072/SupplyChain/dinnig_dk4dxi.webp",
                                 PublicId = "dinnig_dk4dxi",
                                 IsPrimary = true
                  },
                  new ProductPhoto
                  {
                                 PhotoId = 8,
                                 ProductId = 15,
                                 Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1692335025/SupplyChain/cld-sample-5.jpg",
                                 PublicId = "cld-sample-3",
                                 IsPrimary = true
                  },
                          new ProductPhoto
                          {
                              PhotoId = 9,
                              ProductId = 16,
                              Url = "https://res.cloudinary.com/dtryjgzaj/image/upload/v1692335024/SupplyChain/cld-sample-4.jpg",
                              PublicId = "cld-sample-4",
                              IsPrimary = true
                          }
            );


        }
    }
}
