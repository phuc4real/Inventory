using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Database.Migrations
{
    /// <inheritdoc />
    public partial class addColumnExportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Exports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06b302ac-e17a-4b86-9dc8-818f87371585", "phucforeveralone+sa@gmail.com", "PHUCFOREVERALONE+SA@GMAIL.COM", "AQAAAAIAAYagAAAAEBEqDWOL9DuwjX7VQMyDfYM5yXE5BcCkt1eoSKgmTECkZRZpYmAM8Icm/EDbxephpw==", "ddf4cff8-5bfe-40f3-9eae-01fccb355baa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d6675624-097c-4ebc-968d-0a3540af4d1b", "phucforeveralone+ad@gmail.com", "PHUCFOREVERALONE+AD@GMAIL.COM", "AQAAAAIAAYagAAAAEJv93mMrpxdEn1CYFD3CqmVfhevMExamnjSTpTmR5nQKXUQ2VSBWpcXHNVAMTMEAaQ==", "b70158c1-a41a-4fa3-8a13-99336f8a75e4" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "In Review", "review" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Pending", "pending" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Processing", "processing" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Cancel by User", "cancel" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Rejected by Admin", "rejected" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Closed", "close" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 7, "Done", "done" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Exports");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d3afbe88-5469-413e-9dd1-738bb9dc8ee4", "sa@local.com", "SA@LOCAL.COM", "AQAAAAIAAYagAAAAEMeePQdW7RvAVl31NLwmY/THyeiTASGrHrCqioJTHBNbmY6DI0uYGzy3nvEqX9BCFw==", "df3501b4-487f-4339-9924-14a182047907" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "742d45f9-ead1-46a4-a448-6d4c11c72c2c", "admin@local.com", "ADMIN@LOCAL.COM", "AQAAAAIAAYagAAAAEOIvsDwdncA6HGpcgPySZbIOtjiZdwGeCDTOiIdl5rE4vrrZS8GtibUrR7W28YAQ2g==", "bb7d189d-a56a-4003-bc9d-11655d6f5902" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Pending", "pending" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Procesing", "processing" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Cancel", "cancel" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Rejected", "rejected" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Close", "close" });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Done", "done" });
        }
    }
}
