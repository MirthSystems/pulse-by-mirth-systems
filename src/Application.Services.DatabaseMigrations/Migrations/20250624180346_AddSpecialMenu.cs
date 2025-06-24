using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Services.DatabaseMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_venues_venue_categories_category_id",
                table: "venues");

            migrationBuilder.DropTable(
                name: "specials");

            migrationBuilder.DropTable(
                name: "special_categories");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "venues",
                newName: "venue_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_venues_category_id",
                table: "venues",
                newName: "ix_venues_venue_category_id");

            migrationBuilder.CreateTable(
                name: "special_item_categories",
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
                    table.PrimaryKey("pk_special_item_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "specials_menus",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<LocalDate>(type: "date", nullable: false),
                    start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    end_time = table.Column<LocalTime>(type: "time", nullable: true),
                    end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    cron_schedule = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_specials_menus", x => x.id);
                    table.ForeignKey(
                        name: "fk_specials_menus_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "special_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    specials_menu_id = table.Column<long>(type: "bigint", nullable: false),
                    special_item_category_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_special_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_special_items_special_item_categories_special_item_category",
                        column: x => x.special_item_category_id,
                        principalTable: "special_item_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_special_items_specials_menus_specials_menu_id",
                        column: x => x.specials_menu_id,
                        principalTable: "specials_menus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "special_item_categories",
                columns: new[] { "id", "description", "icon", "name", "sort_order" },
                values: new object[,]
                {
                    { 1, "Food specials, appetizers, and meal deals", "🍔", "Food", 1 },
                    { 2, "Drink specials, happy hours, and beverage promotions", "🍺", "Drink", 2 },
                    { 3, "Live music, DJs, trivia, karaoke, and other events", "🎵", "Entertainment", 3 }
                });

            migrationBuilder.InsertData(
                table: "specials_menus",
                columns: new[] { "id", "created_at", "cron_schedule", "description", "end_date", "end_time", "is_active", "is_recurring", "start_date", "start_time", "title", "venue_id" },
                values: new object[,]
                {
                    { 1L, NodaTime.Instant.FromUnixTimeTicks(17507882249137982L), "0 21 * * 5,6", "Live music showcasing the best in local, regional, and national talent. Various genres from rock to jazz.", null, new NodaTime.LocalTime(23, 0), true, true, new NodaTime.LocalDate(2025, 5, 3), new NodaTime.LocalTime(21, 0), "Live Music Weekend", 1L },
                    { 2L, NodaTime.Instant.FromUnixTimeTicks(17507882249139311L), "0 16 * * 1-5", "Enjoy $1 off all draft beers and $5 house wines.", null, new NodaTime.LocalTime(18, 0), true, true, new NodaTime.LocalDate(2025, 5, 1), new NodaTime.LocalTime(16, 0), "Happy Hour", 1L },
                    { 3L, NodaTime.Instant.FromUnixTimeTicks(17507882249139731L), null, "Sweet and spicy chicken sandwich with sweet n spicy sauce, lettuce, and pickles.", new NodaTime.LocalDate(2025, 5, 27), new NodaTime.LocalTime(22, 0), true, false, new NodaTime.LocalDate(2025, 5, 20), new NodaTime.LocalTime(11, 0), "Weekly Food Special", 2L },
                    { 4L, NodaTime.Instant.FromUnixTimeTicks(17507882249139737L), "0 21 * * 3", "Pub Trivia with first and second place prizes. Sponsored by Bacardi Oakheart.", null, new NodaTime.LocalTime(23, 0), true, true, new NodaTime.LocalDate(2025, 5, 22), new NodaTime.LocalTime(21, 0), "Wednesday Night Quizzo", 2L },
                    { 5L, NodaTime.Instant.FromUnixTimeTicks(17507882249139776L), "0 11 * * 2", "Every Tuesday is Mug Club Night. Our valued Mug club members enjoy their First beer, of their choice, on US!!", null, new NodaTime.LocalTime(23, 0), true, true, new NodaTime.LocalDate(2025, 5, 21), new NodaTime.LocalTime(11, 0), "Mug Club Tuesday", 2L },
                    { 6L, NodaTime.Instant.FromUnixTimeTicks(17507882249139779L), "0 10 * * 0", "Special brunch menu served from 10am to 2pm every Sunday.", null, new NodaTime.LocalTime(14, 0), true, true, new NodaTime.LocalDate(2025, 5, 18), new NodaTime.LocalTime(10, 0), "Sunday Brunch", 3L },
                    { 7L, NodaTime.Instant.FromUnixTimeTicks(17507882249139782L), "0 16 * * 2-6", "Enjoy our specially crafted cocktails at a reduced price.", null, new NodaTime.LocalTime(18, 0), true, true, new NodaTime.LocalDate(2025, 5, 21), new NodaTime.LocalTime(16, 0), "Cocktail Hour", 3L }
                });

            migrationBuilder.InsertData(
                table: "special_items",
                columns: new[] { "id", "description", "is_active", "name", "special_item_category_id", "specials_menu_id" },
                values: new object[,]
                {
                    { 1L, "Live music showcasing the best in local, regional, and national talent. Various genres from rock to jazz.", true, "Live Music Friday/Saturday", 3, 1L },
                    { 2L, "Enjoy $1 off all draft beers during happy hour.", true, "$1 Off Draft Beers", 2, 2L },
                    { 3L, "All house wines for just $5 during happy hour.", true, "$5 House Wines", 2, 2L },
                    { 4L, "Sweet and spicy chicken sandwich with sweet n spicy sauce, lettuce, and pickles.", true, "Sweet & Spicy Chicken Sandwich", 1, 3L },
                    { 5L, "Pub Trivia with first and second place prizes. Sponsored by Bacardi Oakheart.", true, "Pub Trivia", 3, 4L },
                    { 6L, "Our valued Mug club members enjoy their First beer, of their choice, on US!!", true, "Free First Beer for Mug Club Members", 2, 5L },
                    { 7L, "Special brunch menu served from 10am to 2pm every Sunday.", true, "Sunday Brunch Menu", 1, 6L },
                    { 8L, "Enjoy our specially crafted cocktails at a reduced price.", true, "Discounted Craft Cocktails", 2, 7L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_special_item_categories_name",
                table: "special_item_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_special_items_special_item_category_id",
                table: "special_items",
                column: "special_item_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_special_items_specials_menu_id",
                table: "special_items",
                column: "specials_menu_id");

            migrationBuilder.CreateIndex(
                name: "ix_specials_menus_venue_id",
                table: "specials_menus",
                column: "venue_id");

            migrationBuilder.AddForeignKey(
                name: "fk_venues_venue_categories_venue_category_id",
                table: "venues",
                column: "venue_category_id",
                principalTable: "venue_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_venues_venue_categories_venue_category_id",
                table: "venues");

            migrationBuilder.DropTable(
                name: "special_items");

            migrationBuilder.DropTable(
                name: "special_item_categories");

            migrationBuilder.DropTable(
                name: "specials_menus");

            migrationBuilder.RenameColumn(
                name: "venue_category_id",
                table: "venues",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "ix_venues_venue_category_id",
                table: "venues",
                newName: "ix_venues_category_id");

            migrationBuilder.CreateTable(
                name: "special_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_special_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "specials",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    special_category_id = table.Column<int>(type: "integer", nullable: false),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    cron_schedule = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    end_time = table.Column<LocalTime>(type: "time", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<LocalDate>(type: "date", nullable: false),
                    start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_specials", x => x.id);
                    table.ForeignKey(
                        name: "fk_specials_special_categories_special_category_id",
                        column: x => x.special_category_id,
                        principalTable: "special_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_specials_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "special_categories",
                columns: new[] { "id", "description", "icon", "name", "sort_order" },
                values: new object[,]
                {
                    { 1, "Food specials, appetizers, and meal deals", "🍔", "Food", 1 },
                    { 2, "Drink specials, happy hours, and beverage promotions", "🍺", "Drink", 2 },
                    { 3, "Live music, DJs, trivia, karaoke, and other events", "🎵", "Entertainment", 3 }
                });

            migrationBuilder.InsertData(
                table: "specials",
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
                name: "ix_special_categories_name",
                table: "special_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_specials_special_category_id",
                table: "specials",
                column: "special_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_specials_venue_id",
                table: "specials",
                column: "venue_id");

            migrationBuilder.AddForeignKey(
                name: "fk_venues_venue_categories_category_id",
                table: "venues",
                column: "category_id",
                principalTable: "venue_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
