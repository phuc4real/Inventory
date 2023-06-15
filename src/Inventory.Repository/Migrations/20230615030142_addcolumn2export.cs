using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addcolumn2export : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "Exports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "Exports");
        }
    }
}
