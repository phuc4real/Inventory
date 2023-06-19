using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addcolumn2exportdetail2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetail_AspNetUsers_ForUserId",
                table: "ExportDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetail_Exports_ExportId",
                table: "ExportDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetail_Items_ItemId",
                table: "ExportDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExportDetail",
                table: "ExportDetail");

            migrationBuilder.RenameTable(
                name: "ExportDetail",
                newName: "ExportDetails");

            migrationBuilder.RenameIndex(
                name: "IX_ExportDetail_ItemId",
                table: "ExportDetails",
                newName: "IX_ExportDetails_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ExportDetail_ForUserId",
                table: "ExportDetails",
                newName: "IX_ExportDetails_ForUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExportDetails",
                table: "ExportDetails",
                columns: new[] { "ExportId", "ItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetails_AspNetUsers_ForUserId",
                table: "ExportDetails",
                column: "ForUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetails_Exports_ExportId",
                table: "ExportDetails",
                column: "ExportId",
                principalTable: "Exports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetails_Items_ItemId",
                table: "ExportDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetails_AspNetUsers_ForUserId",
                table: "ExportDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetails_Exports_ExportId",
                table: "ExportDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetails_Items_ItemId",
                table: "ExportDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExportDetails",
                table: "ExportDetails");

            migrationBuilder.RenameTable(
                name: "ExportDetails",
                newName: "ExportDetail");

            migrationBuilder.RenameIndex(
                name: "IX_ExportDetails_ItemId",
                table: "ExportDetail",
                newName: "IX_ExportDetail_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ExportDetails_ForUserId",
                table: "ExportDetail",
                newName: "IX_ExportDetail_ForUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExportDetail",
                table: "ExportDetail",
                columns: new[] { "ExportId", "ItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetail_AspNetUsers_ForUserId",
                table: "ExportDetail",
                column: "ForUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetail_Exports_ExportId",
                table: "ExportDetail",
                column: "ExportId",
                principalTable: "Exports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetail_Items_ItemId",
                table: "ExportDetail",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
