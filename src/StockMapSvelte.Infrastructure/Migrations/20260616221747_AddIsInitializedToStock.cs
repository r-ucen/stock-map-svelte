using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMapSvelte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInitializedToStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInitialized",
                table: "Stock",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-9dbd-08de40aae72b"),
                column: "IsInitialized",
                value: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-a702-08de40aae72b"),
                column: "IsInitialized",
                value: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-a70b-08de40aae72b"),
                column: "IsInitialized",
                value: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-a70e-08de40aae72b"),
                column: "IsInitialized",
                value: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-a712-08de40aae72b"),
                column: "IsInitialized",
                value: false);

            migrationBuilder.UpdateData(
                table: "Stock",
                keyColumn: "Id",
                keyValue: new Guid("9a480000-0389-c018-a7c2-08de40aae72b"),
                column: "IsInitialized",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInitialized",
                table: "Stock");
        }
    }
}
