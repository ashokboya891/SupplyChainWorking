namespace SupplyChain.Services
{
    using Microsoft.Data.SqlClient;
    using SupplyChain.DTOs;
    using System.Data;

    public class OrderService
    {
        public static SqlParameter GetOrderItemsTVP(List<OrderItemDto> items)
        {
            var table = new DataTable();
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("UnitPrice", typeof(decimal));
            //table.Columns.Add("Status", typeof(string));


            foreach (var item in items)
            {
                table.Rows.Add(item.ProductId, item.Quantity, item.UnitPrice);
            }

            var parameter = new SqlParameter("@OrderItems", SqlDbType.Structured)
            {
                TypeName = "dbo.OrderItemType", // Ensure this matches your TVP definition in SQL
                Value = table
            };

            return parameter;
        }
    }

}
