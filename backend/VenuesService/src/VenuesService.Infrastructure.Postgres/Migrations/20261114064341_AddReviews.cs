using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenuesService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    venue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Published"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                    table.CheckConstraint("ck_reviews_rating", "rating BETWEEN 1 AND 5");
                    table.CheckConstraint("ck_reviews_text_length", "text IS NULL OR char_length(text) <= 2000");
                    table.ForeignKey(
                        name: "fk_reviews_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_author_id_created_at",
                table: "reviews",
                columns: new[] { "author_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_venue_id_rating",
                table: "reviews",
                columns: new[] { "venue_id", "rating" });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_venue_id_status_created_at",
                table: "reviews",
                columns: new[] { "venue_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_reviews_venue_id_author_id",
                table: "reviews",
                columns: new[] { "venue_id", "author_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reviews");
        }
    }
}
