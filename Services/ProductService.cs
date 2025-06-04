using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using SupplyChain.DTOs;
using SupplyChain.IRepoContracts;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;
using System.Data;


namespace SupplyChain.Services
{
    public class ProductService:IProductService
    {

        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository ProductRepository) 
        {
            _productRepository =  ProductRepository; // Assuming you have a concrete implementation of IProductRepository
        }


        public static SqlParameter GetOrderItemsTVP(List<OrderItemDto> items)
        {
            var table = new DataTable();
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("UnitPrice", typeof(decimal));

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
                            Price = price
                        };

                        await _productRepository.AddProductAsync(newProduct);
                        productsInserted++;
                    }
                }
            }

            return productsInserted;

        }

    }

}
