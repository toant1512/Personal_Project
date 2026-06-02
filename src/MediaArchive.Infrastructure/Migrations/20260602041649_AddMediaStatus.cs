using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaArchive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MediaItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MediaItems");
        }
    }
}
