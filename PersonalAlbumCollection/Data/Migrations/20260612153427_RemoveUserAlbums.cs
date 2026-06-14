using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalAlbumCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAlbums");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAlbums",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AlbumId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Review = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlbums", x => new { x.UserId, x.AlbumId });
                    table.CheckConstraint("CK_UserAlbum_Score_1_10", "\"Score\" IS NULL OR (\"Score\" BETWEEN 1 AND 10)");
                    table.ForeignKey(
                        name: "FK_UserAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAlbums_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAlbums_AlbumId",
                table: "UserAlbums",
                column: "AlbumId");
        }
    }
}
