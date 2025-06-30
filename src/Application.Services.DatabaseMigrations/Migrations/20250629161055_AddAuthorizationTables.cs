using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Services.DatabaseMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorizationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sub = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_venue_permissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    granted_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    granted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_venue_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_venue_permissions_users_granted_by_user_id",
                        column: x => x.granted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_venue_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_venue_permissions_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venue_invitations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    venue_id = table.Column<long>(type: "bigint", nullable: false),
                    permission = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    invited_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    invited_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    accepted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    accepted_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venue_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_venue_invitations_users_accepted_by_user_id",
                        column: x => x.accepted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_venue_invitations_users_invited_by_user_id",
                        column: x => x.invited_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_venue_invitations_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_venue_permissions_granted_by_user_id",
                table: "user_venue_permissions",
                column: "granted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_venue_permissions_is_active",
                table: "user_venue_permissions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_user_venue_permissions_user_venue",
                table: "user_venue_permissions",
                columns: new[] { "user_id", "venue_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_venue_permissions_venue_id",
                table: "user_venue_permissions",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_venue_permissions_venue_role_active",
                table: "user_venue_permissions",
                columns: new[] { "venue_id", "name", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_is_active",
                table: "users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_users_sub",
                table: "users",
                column: "sub",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_venue_invitations_accepted_by_user_id",
                table: "venue_invitations",
                column: "accepted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_venue_invitations_active_accepted",
                table: "venue_invitations",
                columns: new[] { "is_active", "accepted_at" });

            migrationBuilder.CreateIndex(
                name: "IX_venue_invitations_email_venue_active",
                table: "venue_invitations",
                columns: new[] { "email", "venue_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_venue_invitations_expires_at",
                table: "venue_invitations",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_venue_invitations_invited_by",
                table: "venue_invitations",
                column: "invited_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_venue_invitations_venue_id",
                table: "venue_invitations",
                column: "venue_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_venue_permissions");

            migrationBuilder.DropTable(
                name: "venue_invitations");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
