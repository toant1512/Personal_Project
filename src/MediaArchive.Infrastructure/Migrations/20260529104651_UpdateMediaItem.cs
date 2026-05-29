using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaArchive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "MediaItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "MediaItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_UserId",
                table: "MediaItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaItems_Users_UserId",
                table: "MediaItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaItems_Users_UserId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_UserId",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "MediaItems");
        }
    }
}
