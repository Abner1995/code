using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactPhoneTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_T_User",
                table: "T_User");

            migrationBuilder.RenameTable(
                name: "T_User",
                newName: "t_user");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_user",
                table: "t_user",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "t_contactdomainentities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    UserName = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, comment: "姓名")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_contactdomainentities", x => x.Id);
                },
                comment: "联系人表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_phone",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false, comment: "联系人ID"),
                    UserId = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    Mobile = table.Column<string>(type: "longtext", nullable: false, comment: "联系号码")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                },
                comment: "联系号码表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_contactdomainentities");

            migrationBuilder.DropTable(
                name: "t_phone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_user",
                table: "t_user");

            migrationBuilder.RenameTable(
                name: "t_user",
                newName: "T_User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_User",
                table: "T_User",
                column: "Id");
        }
    }
}
