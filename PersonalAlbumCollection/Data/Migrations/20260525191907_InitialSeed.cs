using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalAlbumCollection.Data.Seed
{
    /// <inheritdoc />
    public partial class InitialSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
