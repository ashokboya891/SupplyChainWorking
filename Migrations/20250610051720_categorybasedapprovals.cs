using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupplyChain.Migrations
{
    /// <inheritdoc />
    public partial class categorybasedapprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryApprovalStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryApprovalStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryApprovalStages_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Hardware" },
                    { 2, "Software" },
                    { 3, "Miscellaneous" },
                    {4, "Office Supplies" },
                    { 5, "Electronics" },
                    { 6, "Furniture"   },
                    { 7, "Tools" },
                    { 8, "Safety Equipment" },
                    { 9, "Cleaning Supplies" },
                    { 10, "Stationery" }
                });

            migrationBuilder.InsertData(
                table: "CategoryApprovalStages",
                columns: new[] { "Id", "CategoryId", "Role", "StageOrder" },
                values: new object[,]
                {
                    { 1, 1, 2, 1 },
                    { 2, 1, 3, 2 },
                    { 3, 2, 1, 1 },
                    { 4, 2, 3, 2 },
                    { 5, 3, 2, 1 },
                    { 6, 6, 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryApprovalStages_CategoryId",
                table: "CategoryApprovalStages",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryApprovalStages");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Requests");
        }
    }
}
