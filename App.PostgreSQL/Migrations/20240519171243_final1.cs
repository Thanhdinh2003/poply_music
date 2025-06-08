using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.PostgreSQL.Migrations
{
    public partial class final1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media_interactions");

            migrationBuilder.CreateIndex(
                name: "IX_favourite_collections_MediaId",
                table: "favourite_collections",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_favourite_collections_media_MediaId",
                table: "favourite_collections",
                column: "MediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favourite_collections_users_UserId",
                table: "favourite_collections",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favourite_collections_media_MediaId",
                table: "favourite_collections");

            migrationBuilder.DropForeignKey(
                name: "FK_favourite_collections_users_UserId",
                table: "favourite_collections");

            migrationBuilder.DropIndex(
                name: "IX_favourite_collections_MediaId",
                table: "favourite_collections");

            migrationBuilder.CreateTable(
                name: "media_interactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    MediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_interactions", x => x.Id);
                });
        }
    }
}
