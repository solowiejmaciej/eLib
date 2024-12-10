using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLib.Migrations
{
    /// <inheritdoc />
    public partial class ReadingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadingListEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingListEntries_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingListEntries_BookId",
                table: "ReadingListEntries",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingListEntries_UserId",
                table: "ReadingListEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingListEntries_UserId_BookId",
                table: "ReadingListEntries",
                columns: new[] { "UserId", "BookId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingListEntries");
        }
    }
}
