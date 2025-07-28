using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.IRepoContracts;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;
using System.Data;


namespace SupplyChain.Services
{
    public class ProductService:IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository ProductRepository,ApplicationDbContext con)
        {
            _context = con;
            _productRepository = ProductRepository; // Assuming you have a concrete implementation of IProductRepository
        }


        public static SqlParameter GetOrderItemsTVP(List<OrderItemDto> items)
        {
            var table = new DataTable();
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("UnitPrice", typeof(decimal));
            //table.Columns.Add("UnitPrice", typeof(decimal));
            //table.Columns.Add("UnitPrice", typeof(decimal));


            foreach (var item in items)
            {
                table.Rows.Add(item.ProductId, item.Quantity, item.UnitPrice);
            }

            var parameter = new SqlParameter("@OrderItems", table)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.OrderItemType"
            };

            return parameter;
        }

        public async Task<int> UploadOrdersFromExcelFile(IFormFile formfile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formfile.CopyToAsync(memoryStream);

            int productsInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Products"];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Skip header
                {
                    string? name = worksheet.Cells[row, 1].Value?.ToString();
                    string? stockStr = worksheet.Cells[row, 2].Value?.ToString();
                    string? thresholdStr = worksheet.Cells[row, 3].Value?.ToString();
                    string? priceStr = worksheet.Cells[row, 4].Value?.ToString();
                    string? Description = worksheet.Cells[row, 5].Value?.ToString();
                    string? ProductBrandId = worksheet.Cells[row, 6].Value?.ToString();
                    string? ProductTypeId = worksheet.Cells[row, 7].Value?.ToString();




                    if (!string.IsNullOrWhiteSpace(name) &&
                        int.TryParse(stockStr, out int currentStock) &&
                        int.TryParse(thresholdStr, out int threshold) &&
                        decimal.TryParse(priceStr, out decimal price))
                    {
                        var newProduct = new Product
                        {
                            Name = name,
                            CurrentStock = currentStock,
                            Threshold = threshold,
                            Price = price,
                            Description = Description ??    "not added description", // Ensure Description is not null
                            ProductBrandId = int.TryParse(ProductBrandId, out int brandId) ? brandId : 0,
                            ProductTypeId = int.TryParse(ProductTypeId, out int typeId) ? typeId : 0
                        };

                        await _productRepository.AddProductAsync(newProduct);
                        productsInserted++;
                    }
                }
            }

            return productsInserted;

        }


        public async Task<ProductToReturnDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductToReturnDto
            {
                Id = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductBrand = product.ProductBrand?.Name,
                ProductType = product.ProductType?.Name,
                CurrentStock = product.CurrentStock.ToString(),
                PrimaryImageUrl = product.Photos?.FirstOrDefault(p => p.IsPrimary)?.Url,
                AllImages = product.Photos?.Select(p => p.Url).ToList()
            };
        }

        public async Task<bool> AddProduct(Product product)
        {
            await _productRepository.AddProductAsync(product);
            return await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
            return await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return false;

            await _productRepository.DeleteProductAsync(product);
            return await _productRepository.SaveChangesAsync();
        }
        public async Task<ProductMetaDataDto> GetProductMetaDataAsync()
        {
            var brands = await _context.ProductBrands
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name })
                .ToListAsync();

            var types = await _context.ProductTypes
                .Select(t => new TypeDto { Id = t.Id, Name = t.Name })
                .ToListAsync();

            return new ProductMetaDataDto
            {
                Brands = brands,
                Types = types
            };
        }
        public async Task<Product> GetProductEntityByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> GetProductPhotosByIdAsync(int id)
        {
            return await _context.Products.Include(p=>p.Photos).FirstOrDefaultAsync(p => p.ProductId == id);
        }

    }

}
