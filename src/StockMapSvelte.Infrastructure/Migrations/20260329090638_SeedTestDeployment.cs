using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMapSvelte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestDeployment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DeploymentTest",
                columns: new[] { "Id", "CreatedAt", "TestValue" },
                values: new object[] { 1, new DateTime(2026, 3, 29, 9, 6, 36, 930, DateTimeKind.Utc).AddTicks(1772), "Test" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeploymentTest",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
