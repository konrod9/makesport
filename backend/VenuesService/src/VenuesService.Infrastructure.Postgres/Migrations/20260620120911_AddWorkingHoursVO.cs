using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenuesService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingHoursVO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "working_hours",
                table: "venues");

            migrationBuilder.AddColumn<string>(
                name: "working_end",
                table: "venues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "working_start",
                table: "venues",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "working_end",
                table: "venues");

            migrationBuilder.DropColumn(
                name: "working_start",
                table: "venues");

            migrationBuilder.AddColumn<string>(
                name: "working_hours",
                table: "venues",
                type: "text",
                nullable: true);
        }
    }
}
