using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.PostgreSQL.Migrations
{
    public partial class addAuthorAvatarCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "authors",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "authors");
        }
    }
}
