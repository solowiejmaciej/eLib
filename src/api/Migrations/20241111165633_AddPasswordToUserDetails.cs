using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLib.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToUserDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserDetails");
        }
    }
}
