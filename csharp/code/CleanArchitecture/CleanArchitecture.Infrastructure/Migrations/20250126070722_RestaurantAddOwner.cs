using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantAddOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "T_Restaurant",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE T_Restaurant " +
                "SET OwnerId = (SELECT TOP 1 Id FROM AspNetUsers)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Restaurant_OwnerId",
                table: "T_Restaurant",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Restaurant_AspNetUsers_OwnerId",
                table: "T_Restaurant",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Restaurant_AspNetUsers_OwnerId",
                table: "T_Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_T_Restaurant_OwnerId",
                table: "T_Restaurant");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "T_Restaurant");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
