using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addnewRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "fc2a7273-a3c2-47be-bc55-aab11097e09a", null, "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshTokenExpireTime", "SecurityStamp", "TeamId", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d2f7a36c-d4a6-43db-8fe9-74598da4c352", 0, "c131f937-f677-4a7a-b89f-ee9639e14dff", "AppUser", "admin@local.com", false, null, null, false, null, "ADMIN@LOCAL.COM", "ADMIN", "AQAAAAIAAYagAAAAEPa/xqE1JzdLWUXukv9ZKd+aa/GMUHw2D36mGC4cyIiiKbA0VojuXdvpP13aLS/+eQ==", null, false, null, "0f91f666-b26d-4f07-8463-0e51158e28ce", null, false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "fc2a7273-a3c2-47be-bc55-aab11097e09a", "d2f7a36c-d4a6-43db-8fe9-74598da4c352" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "fc2a7273-a3c2-47be-bc55-aab11097e09a", "d2f7a36c-d4a6-43db-8fe9-74598da4c352" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fc2a7273-a3c2-47be-bc55-aab11097e09a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d2f7a36c-d4a6-43db-8fe9-74598da4c352");
        }
    }
}
