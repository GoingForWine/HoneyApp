using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class SeedDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Linden" },
                    { 2, "Bio" },
                    { 3, "Sunflower" },
                    { 4, "Bouquet" },
                    { 5, "Honeydew" }
                });

            migrationBuilder.InsertData(
                table: "Flavours",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Natural" },
                    { 2, "Strawberry" },
                    { 3, "Mint and Ginger" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "Content", "ImageUrl", "IsActive", "Title" },
                values: new object[] { new Guid("7b55a828-68be-45e8-9991-0f19cee32622"), new Guid("bd56fe08-bd10-4384-89be-63a211fbbc61"), "Welcome to this new site, I am the first beekeeper hereenjoy your stay.", "https://th.bing.com/th/id/OIP.eYhgoQcmVrOQG4mTZWpdLwHaE6?rs=1&pid=ImgDetMain", true, "The site's first post" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Flavours",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Flavours",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Flavours",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("7b55a828-68be-45e8-9991-0f19cee32622"));
        }
    }
}
