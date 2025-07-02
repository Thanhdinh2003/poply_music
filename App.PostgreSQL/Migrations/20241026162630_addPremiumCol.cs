using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.PostgreSQL.Migrations
{
    public partial class addPremiumCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumAccount",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PremiumUpgradeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumUpgradeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiumUpgradeRequests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremiumUpgradeRequests_UserId",
                table: "PremiumUpgradeRequests",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremiumUpgradeRequests");

            migrationBuilder.DropColumn(
                name: "IsPremiumAccount",
                table: "users");
        }
    }
}
