using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addcolumn2exportdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForUserId",
                table: "ExportDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExportDetail_ForUserId",
                table: "ExportDetail",
                column: "ForUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDetail_AspNetUsers_ForUserId",
                table: "ExportDetail",
                column: "ForUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDetail_AspNetUsers_ForUserId",
                table: "ExportDetail");

            migrationBuilder.DropIndex(
                name: "IX_ExportDetail_ForUserId",
                table: "ExportDetail");

            migrationBuilder.DropColumn(
                name: "ForUserId",
                table: "ExportDetail");
        }
    }
}
