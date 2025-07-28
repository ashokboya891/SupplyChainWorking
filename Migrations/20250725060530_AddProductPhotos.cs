using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupplyChain.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPhoto",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPhoto", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_ProductPhoto_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProductPhoto",
                columns: new[] { "PhotoId", "IsPrimary", "ProductId", "PublicId", "Url" },
                values: new object[,]
                {
                    { 1, true, 1, "wireless_mouse", "https://res.cloudinary.com/your-cloud/image/upload/v162876/wireless_mouse.jpg" },
                    { 2, true, 7, "bluetooth-speaker_egazpg", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/bluetooth-speaker_egazpg.webp" },
                    { 3, true, 2, "keyboard_qisjqv", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/keyboard_qisjqv.webp" },
                    { 4, true, 20, "toaster_ujtplk", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/toaster_ujtplk.webp" },
                    { 5, true, 19, "english_siabfk", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/english_siabfk.webp" },
                    { 6, true, 16, "kettle_bbamzn", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422073/SupplyChain/kettle_bbamzn.webp" },
                    { 7, true, 19, "dinnig_dk4dxi", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1753422072/SupplyChain/dinnig_dk4dxi.webp" },
                    { 8, true, 15, "cld-sample-3", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1692335025/SupplyChain/cld-sample-5.jpg" },
                    { 9, true, 16, "cld-sample-4", "https://res.cloudinary.com/dtryjgzaj/image/upload/v1692335024/SupplyChain/cld-sample-4.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhoto_ProductId",
                table: "ProductPhoto",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPhoto");
        }
    }
}
