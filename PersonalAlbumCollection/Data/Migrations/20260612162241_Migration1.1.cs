using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalAlbumCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genres_Name",
                table: "Genres");

            migrationBuilder.DeleteData(
                table: "AlbumArtists",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AlbumArtists",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "AlbumArtists",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Genres",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Artists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Albums",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_UserId_Name",
                table: "Genres",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_UserId_Name",
                table: "Artists",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Albums_UserId",
                table: "Albums",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Users_UserId",
                table: "Albums",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Users_UserId",
                table: "Artists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Users_UserId",
                table: "Genres",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Users_UserId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Users_UserId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Users_UserId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_UserId_Name",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Artists_UserId_Name",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Albums_UserId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Albums");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "CoverUrl", "CreatedAt", "Description", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ikoniczny album popowy, uznawany za jeden z najlepiej sprzedajacych sie w historii.", 1982, "Thriller" },
                    { 2, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Klasyczny album The Beatles, znany m.in. z utworu \"Come Together\".", 1969, "Abbey Road" },
                    { 3, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Solowy album Freddie'go Mercury'ego, prezentujacy jego indywidualny styl.", 1985, "Mr. Bad Guy" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "ArtistType", "Country", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "USA", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Krol popu, znany z albumu \"Thriller\" i widowiskowych wystepow.", "Michael Jackson" },
                    { 2, 2, "Wielka Brytania", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Legendarny zespol z Liverpoolu, ktory zrewolucjonizowal muzyke lat 60.", "The Beatles" },
                    { 3, 1, "Wielka Brytania", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Charyzmatyczny wokalista Queen i autor wielu klasycznych utworow.", "Freddie Mercury" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Rock" },
                    { 2, "Pop" },
                    { 3, "Rap" },
                    { 4, "Hip Hop" },
                    { 5, "Jazz" },
                    { 6, "Blues" },
                    { 7, "Metal" }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtists",
                columns: new[] { "AlbumId", "ArtistId", "DisplayOrder" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);
        }
    }
}
