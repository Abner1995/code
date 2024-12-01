using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Restaurant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "餐厅名"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "餐厅介绍"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "餐厅分类"),
                    HasDelivery = table.Column<bool>(type: "bit", nullable: false, comment: "是否配送"),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "邮箱"),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "联系号码"),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "城市"),
                    Address_Street = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "街道"),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "邮政编码")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Restaurant", x => x.Id);
                },
                comment: "餐厅");

            migrationBuilder.CreateTable(
                name: "T_Dish",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "菜单名"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "菜单介绍"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "价格"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Dish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Dish_T_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "T_Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "菜单");

            migrationBuilder.CreateIndex(
                name: "IX_T_Dish_RestaurantId",
                table: "T_Dish",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Dish");

            migrationBuilder.DropTable(
                name: "T_Restaurant");
        }
    }
}
