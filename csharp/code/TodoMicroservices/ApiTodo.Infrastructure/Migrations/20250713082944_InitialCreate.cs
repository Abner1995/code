using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTodo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_todos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, comment: "标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<long>(type: "bigint", nullable: true, defaultValue: 0L, comment: "用户ID"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "任务状态: Pending, InProgress, Completed, Cancelled"),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "任务优先级: Low, Medium, High"),
                    DueDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "截至时间"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_todos", x => x.Id);
                },
                comment: "Todos表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_todos");
        }
    }
}
