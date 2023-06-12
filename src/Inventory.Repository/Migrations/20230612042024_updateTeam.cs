using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Lead",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "Lead",
                table: "Teams",
                newName: "LeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_Lead",
                table: "Teams",
                newName: "IX_Teams_LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_LeaderId",
                table: "Teams",
                column: "LeaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_LeaderId",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "LeaderId",
                table: "Teams",
                newName: "Lead");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_LeaderId",
                table: "Teams",
                newName: "IX_Teams_Lead");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_Lead",
                table: "Teams",
                column: "Lead",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
