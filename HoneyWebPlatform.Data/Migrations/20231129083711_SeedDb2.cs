using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class SeedDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BeePollens",
                columns: new[] { "Id", "BeekeeperId", "Description", "ImageUrl", "NetWeight", "Price", "Title", "CreatedOn", "IsActive" },
                values: new object[] { new Guid("88f4ae54-7e7e-4074-a777-0385b3239291"), new Guid("7adaf90e-fec8-492e-8760-fe3190f1d689"), "Asen Asenev's rich in vitamins and minerals bee pollen. It has the riches of bulgarian nature", "https://naturalvita.co.uk/wp-content/uploads/2018/03/bee-pollen-natruralvita-1024x1024.jpg", 100, 9.00m, "Asen's Bee Pollen", DateTime.UtcNow, true });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedOn", "IsActive", "ParentPostId" },
                values: new object[] { new Guid("9ba2104b-2411-4f38-b77e-42d16751aa0d"), new Guid("bd56fe08-bd10-4384-89be-63a211fbbc61"), "This is the first comment on the first post!", new DateTime(2023, 11, 29, 10, 37, 10, 569, DateTimeKind.Local).AddTicks(162), true, new Guid("7b55a828-68be-45e8-9991-0f19cee32622") });

            migrationBuilder.InsertData(
                table: "Honeys",
                columns: new[] { "Id", "BeekeeperId", "CategoryId", "Description", "ImageUrl", "NetWeight", "Origin", "Price", "Title", "YearMade" },
                values: new object[] { new Guid("dddb10e0-c846-4068-87b8-ecca083f63c8"), new Guid("7adaf90e-fec8-492e-8760-fe3190f1d689"), 1, "High quality linden honey from Asen Asenev.", "https://beehoneyportal.com/wp-content/uploads/2014/10/burkan-s-med-3.jpg", 450, "Vratsa", 12.00m, "Asen's Linden Honey", 2022 });

            migrationBuilder.InsertData(
                table: "Propolises",
                columns: new[] { "Id", "BeekeeperId", "Description", "FlavourId", "ImageUrl", "Price", "Title" },
                values: new object[] { new Guid("26cdb5a8-97dc-458f-ad25-00f1c7052bac"), new Guid("7adaf90e-fec8-492e-8760-fe3190f1d689"), "30% tincture that is good for the immune system with anti-inflammatory properties.", 1, "https://www.apihealth.co.nz/wp-content/uploads/2019/07/Propolis-Tincture.jpg", 3.00m, "Bee Propolis" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BeePollens",
                keyColumn: "Id",
                keyValue: new Guid("88f4ae54-7e7e-4074-a777-0385b3239291"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("9ba2104b-2411-4f38-b77e-42d16751aa0d"));

            migrationBuilder.DeleteData(
                table: "Honeys",
                keyColumn: "Id",
                keyValue: new Guid("dddb10e0-c846-4068-87b8-ecca083f63c8"));

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("fba257f2-5da6-4eed-ae0b-236942c3c97b"));

            migrationBuilder.DeleteData(
                table: "Propolises",
                keyColumn: "Id",
                keyValue: new Guid("26cdb5a8-97dc-458f-ad25-00f1c7052bac"));
        }
    }
}
