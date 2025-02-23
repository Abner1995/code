using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKey2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_refreshtoken_t_user_UserId",
                table: "t_refreshtoken");

            migrationBuilder.DropIndex(
                name: "IX_t_refreshtoken_UserId",
                table: "t_refreshtoken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_t_refreshtoken_UserId",
                table: "t_refreshtoken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_refreshtoken_t_user_UserId",
                table: "t_refreshtoken",
                column: "UserId",
                principalTable: "t_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
