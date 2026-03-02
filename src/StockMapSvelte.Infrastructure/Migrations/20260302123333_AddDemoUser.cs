using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMapSvelte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "505aedf9-29e3-4728-a8f0-d295040dbaef", 0, "acc87749-389b-4b5f-980c-0f3a49a37041", "demo@rucen.me", true, true, null, "DEMO@RUCEN.ME", "DEMO@RUCEN.ME", "AQAAAAEAACcQAAAAEF6Y+gXuBs8BQjWlNGbt586Sj20CDUvlsbLiWv+dP/+HawSCt4i+oXRH7g9DWHX+RA==", null, false, "TEJXOSATJKOEB4YFF32Y5G2XPR5OFEL7", false, "demo@rucen.me" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "505aedf9-29e3-4728-a8f0-d295040dbaef");
        }
    }
}
