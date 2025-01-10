using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCloset.Data.Migrations
{
    /// <inheritdoc />
    public partial class publicBookmark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Bookmarks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Bookmarks");
        }
    }
}
