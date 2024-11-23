using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLib.NotificationService.Migrations
{
    /// <inheritdoc />
    public partial class NotificationFailedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAt",
                table: "Notifications");
        }
    }
}
