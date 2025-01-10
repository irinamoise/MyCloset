using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCloset.Data.Migrations
{
    /// <inheritdoc />
    public partial class comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBookmarks_Bookmarks_BookmarkId",
                table: "ItemBookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBookmarks_Items_ItemId",
                table: "ItemBookmarks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemBookmarks",
                table: "ItemBookmarks");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "ItemBookmarks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BookmarkId",
                table: "ItemBookmarks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemBookmarks",
                table: "ItemBookmarks",
                columns: new[] { "Id", "ItemId", "BookmarkId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBookmarks_Bookmarks_BookmarkId",
                table: "ItemBookmarks",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBookmarks_Items_ItemId",
                table: "ItemBookmarks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBookmarks_Bookmarks_BookmarkId",
                table: "ItemBookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBookmarks_Items_ItemId",
                table: "ItemBookmarks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemBookmarks",
                table: "ItemBookmarks");

            migrationBuilder.AlterColumn<int>(
                name: "BookmarkId",
                table: "ItemBookmarks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "ItemBookmarks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemBookmarks",
                table: "ItemBookmarks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Items_ItemId",
                table: "Comments",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBookmarks_Bookmarks_BookmarkId",
                table: "ItemBookmarks",
                column: "BookmarkId",
                principalTable: "Bookmarks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBookmarks_Items_ItemId",
                table: "ItemBookmarks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }
    }
}
