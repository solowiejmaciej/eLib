using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLib.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationChannelToUserDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationChannel",
                table: "UserDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationChannel",
                table: "UserDetails");
        }
    }
}
