using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.PostgreSQL.Migrations
{
    public partial class final2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mediaContent_media_MediaId",
                table: "mediaContent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mediaContent",
                table: "mediaContent");

            migrationBuilder.RenameTable(
                name: "mediaContent",
                newName: "media_content");

            migrationBuilder.RenameIndex(
                name: "IX_mediaContent_MediaId",
                table: "media_content",
                newName: "IX_media_content_MediaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_media_content",
                table: "media_content",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_media_content_media_MediaId",
                table: "media_content",
                column: "MediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_media_content_media_MediaId",
                table: "media_content");

            migrationBuilder.DropPrimaryKey(
                name: "PK_media_content",
                table: "media_content");

            migrationBuilder.RenameTable(
                name: "media_content",
                newName: "mediaContent");

            migrationBuilder.RenameIndex(
                name: "IX_media_content_MediaId",
                table: "mediaContent",
                newName: "IX_mediaContent_MediaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mediaContent",
                table: "mediaContent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mediaContent_media_MediaId",
                table: "mediaContent",
                column: "MediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
