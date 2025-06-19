using System;
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
                name: "application_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "days_of_week",
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
                    table.PrimaryKey("pk_days_of_week", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "venue_categories",
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
                    table.PrimaryKey("pk_venue_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application_role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "application_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "application_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_application_user_logins_application_users_user_id",
                        column: x => x.user_id,
                        principalTable: "application_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_roles",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_application_user_roles_application_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "application_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_user_roles_application_users_user_id",
                        column: x => x.user_id,
                        principalTable: "application_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_tokens",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_application_user_tokens_application_users_user_id",
                        column: x => x.user_id,
                        principalTable: "application_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venues",
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
                    table.PrimaryKey("pk_venues", x => x.id);
                    table.ForeignKey(
                        name: "fk_venues_venue_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "venue_categories",
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
                    is_closed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_hours", x => x.id);
                    table.ForeignKey(
                        name: "fk_business_hours_day_of_weeks_day_of_week_id",
                        column: x => x.day_of_week_id,
                        principalTable: "days_of_week",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_business_hours_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "days_of_week",
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
                table: "venue_categories",
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
                table: "venues",
                columns: new[] { "id", "category_id", "country", "description", "email", "is_active", "locality", "location", "name", "phone_number", "postal_code", "profile_image", "region", "secondary_address", "street_address", "website" },
                values: new object[,]
                {
                    { 1L, 7, "United States", "Local craft brewery featuring house-made beers, pub fare, and live entertainment in a cozy atmosphere.", "info@bullfrogbrewery.com", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0057192 41.240432)"), "Bullfrog Brewery", "(570) 326-4700", "17701", null, "PA", null, "229 W 4th St", "https://bullfrogbrewery.com" },
                    { 2L, 1, "United States", "Family-friendly restaurant and ale house serving American cuisine with a great selection of craft beers and cocktails.", "info@thebrickyard.net", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0037646 41.2409825)"), "The Brickyard Restaurant & Ale House", "(570) 322-3876", "17701", null, "PA", null, "343 Pine St", "https://thebrickyard.net" },
                    { 3L, 2, "United States", "Upscale gastropub featuring craft cocktails, local beers, and elevated bar food in a sophisticated atmosphere.", "info@thecrookedgoose.com", true, "Williamsport", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-77.0047521 41.2407201)"), "The Crooked Goose", "(570) 360-7435", "17701", null, "PA", null, "215 W 4th St", "https://thecrookedgoose.com" }
                });

            migrationBuilder.InsertData(
                table: "business_hours",
                columns: new[] { "id", "close_time", "day_of_week_id", "open_time", "venue_id" },
                values: new object[,]
                {
                    { 1L, new NodaTime.LocalTime(15, 0), 1, new NodaTime.LocalTime(10, 0), 1L },
                    { 2L, new NodaTime.LocalTime(22, 0), 2, new NodaTime.LocalTime(11, 30), 1L },
                    { 3L, new NodaTime.LocalTime(22, 0), 3, new NodaTime.LocalTime(11, 30), 1L },
                    { 4L, new NodaTime.LocalTime(22, 0), 4, new NodaTime.LocalTime(11, 30), 1L },
                    { 5L, new NodaTime.LocalTime(22, 0), 5, new NodaTime.LocalTime(11, 30), 1L },
                    { 6L, new NodaTime.LocalTime(0, 0), 6, new NodaTime.LocalTime(11, 30), 1L },
                    { 7L, new NodaTime.LocalTime(0, 0), 7, new NodaTime.LocalTime(11, 30), 1L },
                    { 8L, new NodaTime.LocalTime(23, 0), 1, new NodaTime.LocalTime(11, 0), 2L },
                    { 9L, new NodaTime.LocalTime(0, 0), 2, new NodaTime.LocalTime(11, 0), 2L },
                    { 10L, new NodaTime.LocalTime(0, 0), 3, new NodaTime.LocalTime(11, 0), 2L },
                    { 11L, new NodaTime.LocalTime(0, 0), 4, new NodaTime.LocalTime(11, 0), 2L },
                    { 12L, new NodaTime.LocalTime(0, 0), 5, new NodaTime.LocalTime(11, 0), 2L },
                    { 13L, new NodaTime.LocalTime(2, 0), 6, new NodaTime.LocalTime(11, 0), 2L },
                    { 14L, new NodaTime.LocalTime(2, 0), 7, new NodaTime.LocalTime(11, 0), 2L },
                    { 15L, new NodaTime.LocalTime(14, 0), 1, new NodaTime.LocalTime(10, 0), 3L }
                });

            migrationBuilder.InsertData(
                table: "business_hours",
                columns: new[] { "id", "close_time", "day_of_week_id", "is_closed", "open_time", "venue_id" },
                values: new object[] { 16L, null, 2, true, null, 3L });

            migrationBuilder.InsertData(
                table: "business_hours",
                columns: new[] { "id", "close_time", "day_of_week_id", "open_time", "venue_id" },
                values: new object[,]
                {
                    { 17L, new NodaTime.LocalTime(21, 0), 3, new NodaTime.LocalTime(11, 0), 3L },
                    { 18L, new NodaTime.LocalTime(21, 0), 4, new NodaTime.LocalTime(11, 0), 3L },
                    { 19L, new NodaTime.LocalTime(21, 0), 5, new NodaTime.LocalTime(11, 0), 3L },
                    { 20L, new NodaTime.LocalTime(22, 0), 6, new NodaTime.LocalTime(11, 0), 3L },
                    { 21L, new NodaTime.LocalTime(22, 0), 7, new NodaTime.LocalTime(11, 0), 3L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_role_claims_role_id",
                table: "application_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "role_name_index",
                table: "application_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_application_user_claims_user_id",
                table: "application_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_user_logins_user_id",
                table: "application_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_user_roles_role_id",
                table: "application_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "email_index",
                table: "application_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "user_name_index",
                table: "application_users",
                column: "normalized_user_name",
                unique: true);

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
                name: "ix_days_of_week_iso_number",
                table: "days_of_week",
                column: "iso_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_venue_categories_name",
                table: "venue_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_venues_category_id",
                table: "venues",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_venues_location",
                table: "venues",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_venues_name",
                table: "venues",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_role_claims");

            migrationBuilder.DropTable(
                name: "application_user_claims");

            migrationBuilder.DropTable(
                name: "application_user_logins");

            migrationBuilder.DropTable(
                name: "application_user_roles");

            migrationBuilder.DropTable(
                name: "application_user_tokens");

            migrationBuilder.DropTable(
                name: "business_hours");

            migrationBuilder.DropTable(
                name: "application_roles");

            migrationBuilder.DropTable(
                name: "application_users");

            migrationBuilder.DropTable(
                name: "days_of_week");

            migrationBuilder.DropTable(
                name: "venues");

            migrationBuilder.DropTable(
                name: "venue_categories");
        }
    }
}
