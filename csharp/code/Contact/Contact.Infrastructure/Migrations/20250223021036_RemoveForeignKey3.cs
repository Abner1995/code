using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKey3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_phone_t_contactdomainentities_ContactId",
                table: "t_phone");

            migrationBuilder.DropIndex(
                name: "IX_t_phone_ContactId",
                table: "t_phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_t_phone_ContactId",
                table: "t_phone",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_phone_t_contactdomainentities_ContactId",
                table: "t_phone",
                column: "ContactId",
                principalTable: "t_contactdomainentities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
