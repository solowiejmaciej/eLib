using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLib.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasEmailNotifications",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "HasSmsNotifications",
                table: "UserDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasEmailNotifications",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSmsNotifications",
                table: "UserDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
