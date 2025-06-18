using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupplyChain.Migrations
{
    /// <inheritdoc />
    public partial class producttypebranddescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductBrandId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProductBrands",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Logitech" },
                    { 2, "IKEA" },
                    { 3, "Oxford" },
                    { 4, "Prestige" },
                    { 5, "HP" }
                });

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Furniture" },
                    { 3, "Educational" },
                    { 4, "Kitchen" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CurrentStock", "Description", "Name", "Price", "ProductBrandId", "ProductTypeId", "Threshold" },
                values: new object[,]
                {
                    { 1, 50, "Ergonomic Bluetooth wireless mouse", "Wireless Mouse", 899m, 1, 1, 10 },
                    { 2, 30, "RGB mechanical keyboard with macro support", "Gaming Keyboard", 1899m, 1, 1, 5 },
                    { 3, 20, "Adjustable mesh back office chair", "Office Chair", 4999m, 2, 2, 3 },
                    { 4, 25, "Minimalist study desk with drawers", "Study Table", 3599m, 2, 2, 5 },
                    { 5, 70, "CBSE Grade 10 Mathematics", "Mathematics Textbook", 399m, 3, 3, 15 },
                    { 6, 50, "Lab workbook with experiment logs", "Science Workbook", 299m, 3, 3, 10 },
                    { 7, 40, "1.5L electric kettle with auto shut-off", "Electric Kettle", 1499m, 4, 4, 8 },
                    { 8, 35, "24cm frying pan with glass lid", "Non-stick Frying Pan", 799m, 4, 4, 6 },
                    { 9, 10, "Wireless printer with duplex printing", "Inkjet Printer", 6599m, 5, 1, 2 },
                    { 10, 50, "Waterproof 15.6-inch laptop backpack", "Laptop Backpack", 999m, 2, 2, 7 },
                    { 11, 80, "200-page graph notebook for math", "Graph Notebook", 199m, 3, 3, 10 },
                    { 12, 30, "Compact 300W kitchen blender", "Hand Blender", 1199m, 4, 4, 5 },
                    { 13, 40, "Noise-cancelling TWS with mic", "Wireless Earbuds", 2499m, 1, 1, 8 },
                    { 14, 15, "5-layer wooden shelf for study room", "Book Shelf", 2499m, 2, 2, 3 },
                    { 15, 60, "Competitive exam prep book for Physics", "Physics Guide", 449m, 3, 3, 12 },
                    { 16, 25, "5L stainless steel cooker", "Steel Pressure Cooker", 2299m, 4, 4, 5 },
                    { 17, 35, "10W portable waterproof speaker", "Bluetooth Speaker", 1999m, 1, 1, 7 },
                    { 18, 8, "4-seater modern dining set", "Dining Table Set", 7999m, 2, 2, 2 },
                    { 19, 55, "Comprehensive English grammar reference", "English Grammar Book", 349m, 3, 3, 10 },
                    { 20, 22, "2-slice toaster with browning control", "Toaster", 999m, 4, 4, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductBrandId",
                table: "Products",
                column: "ProductBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductBrands_ProductBrandId",
                table: "Products",
                column: "ProductBrandId",
                principalTable: "ProductBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductBrands_ProductBrandId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductBrands");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductBrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductBrandId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "Products");
        }
    }
}
