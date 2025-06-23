using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Services.DatabaseMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialsAndSpecialCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "special_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_special_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "special",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    special_category_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<LocalDate>(type: "date", nullable: false),
                    start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    end_time = table.Column<LocalTime>(type: "time", nullable: true),
                    end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    cron_schedule = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_special", x => x.id);
                    table.ForeignKey(
                        name: "fk_special_special_category_special_category_id",
                        column: x => x.special_category_id,
                        principalTable: "special_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_special_venue_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venue",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "special_category",
                columns: new[] { "id", "description", "icon", "name", "sort_order" },
                values: new object[,]
                {
                    { 1, "Food specials, appetizers, and meal deals", "🍔", "Food", 1 },
                    { 2, "Drink specials, happy hours, and beverage promotions", "🍺", "Drink", 2 },
                    { 3, "Live music, DJs, trivia, karaoke, and other events", "🎵", "Entertainment", 3 }
                });

            migrationBuilder.InsertData(
                table: "special",
                columns: new[] { "id", "cron_schedule", "description", "end_date", "end_time", "is_active", "is_recurring", "special_category_id", "start_date", "start_time", "title", "venue_id" },
                values: new object[,]
                {
                    { 1L, "0 21 * * 5,6", "Live music showcasing the best in local, regional, and national talent. Various genres from rock to jazz.", null, new NodaTime.LocalTime(23, 0), true, true, 3, new NodaTime.LocalDate(2025, 5, 3), new NodaTime.LocalTime(21, 0), "Live Music Friday/Saturday", 1L },
                    { 2L, "0 16 * * 1-5", "Enjoy $1 off all draft beers and $5 house wines.", null, new NodaTime.LocalTime(18, 0), true, true, 2, new NodaTime.LocalDate(2025, 5, 1), new NodaTime.LocalTime(16, 0), "Happy Hour", 1L },
                    { 3L, null, "Sweet and spicy chicken sandwich with sweet n spicy sauce, lettuce, and pickles.", new NodaTime.LocalDate(2025, 5, 27), new NodaTime.LocalTime(22, 0), true, false, 1, new NodaTime.LocalDate(2025, 5, 20), new NodaTime.LocalTime(11, 0), "Weekly Burger Special - Sweet & Spicy Chicken Sandwich", 2L },
                    { 4L, "0 21 * * 3", "Pub Trivia with first and second place prizes. Sponsored by Bacardi Oakheart.", null, new NodaTime.LocalTime(23, 0), true, true, 3, new NodaTime.LocalDate(2025, 5, 22), new NodaTime.LocalTime(21, 0), "Wednesday Night Quizzo", 2L },
                    { 5L, "0 11 * * 2", "Every Tuesday is Mug Club Night. Our valued Mug club members enjoy their First beer, of their choice, on US!!", null, new NodaTime.LocalTime(23, 0), true, true, 2, new NodaTime.LocalDate(2025, 5, 21), new NodaTime.LocalTime(11, 0), "Mug Club Tuesday", 2L },
                    { 6L, "0 10 * * 0", "Special brunch menu served from 10am to 2pm every Sunday.", null, new NodaTime.LocalTime(14, 0), true, true, 1, new NodaTime.LocalDate(2025, 5, 18), new NodaTime.LocalTime(10, 0), "Sunday Brunch", 3L },
                    { 7L, "0 16 * * 2-6", "Enjoy our specially crafted cocktails at a reduced price.", null, new NodaTime.LocalTime(18, 0), true, true, 2, new NodaTime.LocalDate(2025, 5, 21), new NodaTime.LocalTime(16, 0), "Cocktail Hour", 3L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_special_special_category_id",
                table: "special",
                column: "special_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_special_venue_id",
                table: "special",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "ix_special_category_name",
                table: "special_category",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "special");

            migrationBuilder.DropTable(
                name: "special_category");
        }
    }
}
