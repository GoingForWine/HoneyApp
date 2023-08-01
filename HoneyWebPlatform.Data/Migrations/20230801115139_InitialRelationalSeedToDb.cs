using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class InitialRelationalSeedToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BeePollens",
                columns: new[] { "Id", "BeekeeperId", "Description", "ImageUrl", "NetWeight", "Price", "Title" },
                values: new object[] { new Guid("e08d70b3-0f2b-4c21-b4f7-46a69713f8a1"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), "Asen Asenev's rich in vitamins and minerals bee pollen. It has the riches of bulgarian nature", "https://naturalvita.co.uk/wp-content/uploads/2018/03/bee-pollen-natruralvita-1024x1024.jpg", 100, 9.00m, "Asen's Bee Pollen" });

            migrationBuilder.InsertData(
                table: "Honeys",
                columns: new[] { "Id", "BeekeeperId", "CategoryId", "Description", "ImageUrl", "NetWeight", "Origin", "Price", "Title", "YearMade" },
                values: new object[] { new Guid("5e337878-8e69-48d8-ab74-3df44f357401"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), 1, "High quality linden honey from Asen Asenev.", "https://beehoneyportal.com/wp-content/uploads/2014/10/burkan-s-med-3.jpg", 450, "Vratsa", 12.00m, "Asen's Linden Honey", 2022 });

            migrationBuilder.InsertData(
                table: "Propolises",
                columns: new[] { "Id", "BeekeeperId", "Description", "FlavourId", "ImageUrl", "Price", "Title" },
                values: new object[] { new Guid("5af6b804-167f-4873-8571-aa242022a45c"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), "30% tincture that is good for the immune system with anti-inflammatory properties.", 1, "https://www.apihealth.co.nz/wp-content/uploads/2019/07/Propolis-Tincture.jpg", 3.00m, "Bee Propolis" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BeePollens",
                keyColumn: "Id",
                keyValue: new Guid("e08d70b3-0f2b-4c21-b4f7-46a69713f8a1"));

            migrationBuilder.DeleteData(
                table: "Honeys",
                keyColumn: "Id",
                keyValue: new Guid("5e337878-8e69-48d8-ab74-3df44f357401"));

            migrationBuilder.DeleteData(
                table: "Propolises",
                keyColumn: "Id",
                keyValue: new Guid("5af6b804-167f-4873-8571-aa242022a45c"));
        }
    }
}
