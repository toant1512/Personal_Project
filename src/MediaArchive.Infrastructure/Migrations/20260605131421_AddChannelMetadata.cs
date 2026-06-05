using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaArchive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelName",
                table: "MediaItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Uploader",
                table: "MediaItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelName",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "Uploader",
                table: "MediaItems");
        }
    }
}
