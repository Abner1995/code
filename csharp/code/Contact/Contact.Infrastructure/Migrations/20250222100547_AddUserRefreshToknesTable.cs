using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRefreshToknesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_refreshtoken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "longtext", nullable: false, comment: "Token")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expiry = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "过期时间"),
                    DeviceId = table.Column<string>(type: "longtext", nullable: false, comment: "设备标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false, comment: "用户ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_refreshtoken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_refreshtoken_t_user_UserId",
                        column: x => x.UserId,
                        principalTable: "t_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "用户RefreshToken")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_phone_ContactId",
                table: "t_phone",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_t_refreshtoken_UserId",
                table: "t_refreshtoken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_phone_t_contactdomainentities_ContactId",
                table: "t_phone",
                column: "ContactId",
                principalTable: "t_contactdomainentities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_phone_t_contactdomainentities_ContactId",
                table: "t_phone");

            migrationBuilder.DropTable(
                name: "t_refreshtoken");

            migrationBuilder.DropIndex(
                name: "IX_t_phone_ContactId",
                table: "t_phone");
        }
    }
}
