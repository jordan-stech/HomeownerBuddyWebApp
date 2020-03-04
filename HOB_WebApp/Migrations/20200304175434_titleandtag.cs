using Microsoft.EntityFrameworkCore.Migrations;

namespace HOB_WebApp.Migrations
{
    public partial class titleandtag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ContentModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ContentModel",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ContentModel");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ContentModel");
        }
    }
}
