using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Services.DatabaseMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:address_standardizer", ",,")
                .Annotation("Npgsql:PostgresExtension:address_standardizer_data_us", ",,")
                .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
                .Annotation("Npgsql:PostgresExtension:plpgsql", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis_raster", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis_sfcgal", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis_tiger_geocoder", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis_topology", ",,");

            migrationBuilder.CreateTable(
                name: "day_of_week",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    short_name = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    iso_number = table.Column<int>(type: "integer", nullable: false),
                    is_weekday = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_day_of_week", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "venue_category",
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
                    table.PrimaryKey("pk_venue_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "venue",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    profile_image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    street_address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    secondary_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    locality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location = table.Column<Point>(type: "geography (point)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venue", x => x.id);
                    table.ForeignKey(
                        name: "fk_venue_venue_category_category_id",
                        column: x => x.category_id,
                        principalTable: "venue_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "business_hours",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    day_of_week_id = table.Column<int>(type: "integer", nullable: false),
                    open_time = table.Column<LocalTime>(type: "time", nullable: true),
                    close_time = table.Column<LocalTime>(type: "time", nullable: true),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_hours", x => x.id);
                    table.ForeignKey(
                        name: "fk_business_hours_day_of_week_day_of_week_id",
                        column: x => x.day_of_week_id,
                        principalTable: "day_of_week",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_business_hours_venue_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venue",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "day_of_week",
                columns: new[] { "id", "is_weekday", "iso_number", "name", "short_name", "sort_order" },
                values: new object[,]
                {
                    { 1, false, 0, "Sunday", "SUN", 1 },
                    { 2, true, 1, "Monday", "MON", 2 },
                    { 3, true, 2, "Tuesday", "TUE", 3 },
                    { 4, true, 3, "Wednesday", "WED", 4 },
                    { 5, true, 4, "Thursday", "THU", 5 },
                    { 6, true, 5, "Friday", "FRI", 6 },
                    { 7, false, 6, "Saturday", "SAT", 7 }
                });

            migrationBuilder.InsertData(
                table: "venue_category",
                columns: new[] { "id", "description", "icon", "name", "sort_order" },
                values: new object[,]
                {
                    { 1, "Dining establishments offering food and beverages", "🍽️", "Restaurant", 1 },
                    { 2, "Venues focused on drinks and nightlife", "🍸", "Bar", 2 },
                    { 3, "Casual spots for coffee and light meals", "☕", "Cafe", 3 },
                    { 4, "Venues for dancing and late-night entertainment", "🪩", "Nightclub", 4 },
                    { 5, "Casual venues with food, drinks, and often live music", "🍺", "Pub", 5 },
                    { 6, "Venues producing wine, offering tastings, food pairings, and live music", "🍷", "Winery", 6 },
                    { 7, "Venues brewing their own beer, often with food and live music", "🍻", "Brewery", 7 },
                    { 9, "Sophisticated venues with cocktails, small plates, and live music", "🛋️", "Lounge", 8 },
                    { 10, "Intimate dining venues with quality food, wine, and occasional live music", "🥂", "Bistro", 9 }
                });

            migrationBuilder.InsertData(
                table: "venue",
                columns: new[] { "id", "category_id", "country", "description", "email", "is_active", "locality", "location", "name", "phone_number", "postal_code", "profile_image", "region", "secondary_address", "street_address", "website" },
                values: new object[,]
                {
                    { 1L, 7, "United States", "Local craft brewery featuring house-made beers, pub fare, and live entertainment in a cozy atmosphere.", "info@bullfrogbrewery.com", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0057192 41.240432)"), "Bullfrog Brewery", "(570) 326-4700", "17701", null, "PA", null, "229 W 4th St", "https://bullfrogbrewery.com" },
                    { 2L, 1, "United States", "Family-friendly restaurant and ale house serving American cuisine with a great selection of craft beers and cocktails.", "info@thebrickyard.net", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0037646 41.2409825)"), "The Brickyard Restaurant & Ale House", "(570) 322-3876", "17701", null, "PA", null, "343 Pine St", "https://thebrickyard.net" },
                    { 3L, 2, "United States", "Upscale gastropub featuring craft cocktails, local beers, and elevated bar food in a sophisticated atmosphere.", "info@thecrookedgoose.com", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0047521 41.2407201)"), "The Crooked Goose", "(570) 360-7435", "17701", null, "PA", null, "215 W 4th St", "https://thecrookedgoose.com" }
                });

            migrationBuilder.InsertData(
                table: "business_hours",
                columns: new[] { "id", "close_time", "day_of_week_id", "is_closed", "open_time", "venue_id" },
                values: new object[,]
                {
                    { 1L, new NodaTime.LocalTime(15, 0), 1, false, new NodaTime.LocalTime(10, 0), 1L },
                    { 2L, new NodaTime.LocalTime(22, 0), 2, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 3L, new NodaTime.LocalTime(22, 0), 3, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 4L, new NodaTime.LocalTime(22, 0), 4, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 5L, new NodaTime.LocalTime(22, 0), 5, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 6L, new NodaTime.LocalTime(0, 0), 6, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 7L, new NodaTime.LocalTime(0, 0), 7, false, new NodaTime.LocalTime(11, 30), 1L },
                    { 8L, new NodaTime.LocalTime(23, 0), 1, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 9L, new NodaTime.LocalTime(0, 0), 2, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 10L, new NodaTime.LocalTime(0, 0), 3, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 11L, new NodaTime.LocalTime(0, 0), 4, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 12L, new NodaTime.LocalTime(0, 0), 5, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 13L, new NodaTime.LocalTime(2, 0), 6, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 14L, new NodaTime.LocalTime(2, 0), 7, false, new NodaTime.LocalTime(11, 0), 2L },
                    { 15L, new NodaTime.LocalTime(14, 0), 1, false, new NodaTime.LocalTime(10, 0), 3L },
                    { 16L, null, 2, true, null, 3L },
                    { 17L, new NodaTime.LocalTime(21, 0), 3, false, new NodaTime.LocalTime(11, 0), 3L },
                    { 18L, new NodaTime.LocalTime(21, 0), 4, false, new NodaTime.LocalTime(11, 0), 3L },
                    { 19L, new NodaTime.LocalTime(21, 0), 5, false, new NodaTime.LocalTime(11, 0), 3L },
                    { 20L, new NodaTime.LocalTime(22, 0), 6, false, new NodaTime.LocalTime(11, 0), 3L },
                    { 21L, new NodaTime.LocalTime(22, 0), 7, false, new NodaTime.LocalTime(11, 0), 3L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_business_hours_day_of_week_id",
                table: "business_hours",
                column: "day_of_week_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_hours_venue_id_day_of_week_id",
                table: "business_hours",
                columns: new[] { "venue_id", "day_of_week_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_day_of_week_iso_number",
                table: "day_of_week",
                column: "iso_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_venue_category_id",
                table: "venue",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_venue_location",
                table: "venue",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_venue_name",
                table: "venue",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_venue_category_name",
                table: "venue_category",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_hours");

            migrationBuilder.DropTable(
                name: "day_of_week");

            migrationBuilder.DropTable(
                name: "venue");

            migrationBuilder.DropTable(
                name: "venue_category");
        }
    }
}
