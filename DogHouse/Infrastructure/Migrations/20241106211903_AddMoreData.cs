using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DogHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dogs",
                columns: new[] { "Id", "Colors", "Name", "TailLength", "Weight" },
                values: new object[,]
                {
                    { 3, "[\"Brown\"]", "Max", 15.0, 25.0 },
                    { 4, "[\"Golden\"]", "Bella", 20.0, 28.0 },
                    { 5, "[\"Black\"]", "Charlie", 18.0, 35.0 },
                    { 6, "[\"White\"]", "Lucy", 10.0, 22.0 },
                    { 7, "[\"Gray\"]", "Cooper", 12.0, 27.0 },
                    { 8, "[\"Brown\",\"White\"]", "Daisy", 14.0, 24.0 },
                    { 9, "[\"Black\",\"Brown\"]", "Rocky", 16.0, 29.0 },
                    { 10, "[\"Golden\",\"White\"]", "Molly", 19.0, 26.0 },
                    { 11, "[\"Red\"]", "Buddy", 21.0, 31.0 },
                    { 12, "[\"Amber\"]", "Lola", 13.0, 23.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Dogs",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
