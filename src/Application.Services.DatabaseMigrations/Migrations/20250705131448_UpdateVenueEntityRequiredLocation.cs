using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Application.Services.DatabaseMigrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVenueEntityRequiredLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "location",
                table: "venues",
                type: "geography (point)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "timezone_id",
                table: "venues",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "venues",
                keyColumn: "id",
                keyValue: 1L,
                column: "timezone_id",
                value: "America/New_York");

            migrationBuilder.UpdateData(
                table: "venues",
                keyColumn: "id",
                keyValue: 2L,
                column: "timezone_id",
                value: "America/New_York");

            migrationBuilder.UpdateData(
                table: "venues",
                keyColumn: "id",
                keyValue: 3L,
                column: "timezone_id",
                value: "America/New_York");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timezone_id",
                table: "venues");

            migrationBuilder.AlterColumn<Point>(
                name: "location",
                table: "venues",
                type: "geography (point)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography (point)");
        }
    }
}
