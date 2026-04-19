using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMapSvelte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoreMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EarningsGrowth",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EarningsQuarterlyGrowth",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ForwardEps",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FreeCashflow",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OneYearChange",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PegRatio",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ProfitMargins",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RevenueGrowth",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetHighPrice",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetLowPrice",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetMeanPrice",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetMedianPrice",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalDebt",
                table: "StockProfile",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TrailingEps",
                table: "StockProfile",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarningsGrowth",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "EarningsQuarterlyGrowth",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "ForwardEps",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "FreeCashflow",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "OneYearChange",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "PegRatio",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "ProfitMargins",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "RevenueGrowth",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TargetHighPrice",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TargetLowPrice",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TargetMeanPrice",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TargetMedianPrice",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TotalDebt",
                table: "StockProfile");

            migrationBuilder.DropColumn(
                name: "TrailingEps",
                table: "StockProfile");
        }
    }
}
