using Microsoft.EntityFrameworkCore.Migrations;

namespace HOB_WebApp.Migrations
{
    public partial class code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeCode",
                table: "MobileUsers");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MobileUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "MobileUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HomeCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeCodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeCodes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "MobileUsers");

            migrationBuilder.DropColumn(
                name: "address",
                table: "MobileUsers");

            migrationBuilder.AddColumn<string>(
                name: "HomeCode",
                table: "MobileUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
