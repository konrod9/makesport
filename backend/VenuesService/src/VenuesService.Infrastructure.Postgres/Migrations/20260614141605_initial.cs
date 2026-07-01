using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace VenuesService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_open = table.Column<bool>(type: "boolean", nullable: false),
                    has_lighting = table.Column<bool>(type: "boolean", nullable: false),
                    is_free = table.Column<bool>(type: "boolean", nullable: false),
                    working_hours = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    review_count = table.Column<int>(type: "integer", nullable: false),
                    sport_type = table.Column<string>(type: "text", nullable: false),
                    surface = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    building = table.Column<string>(type: "text", nullable: true),
                    coordinates = table.Column<Point>(type: "geography (Point,4326)", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venues", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "venues");
        }
    }
}
