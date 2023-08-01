using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class AddingAndSeedingPostEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BeePollens",
                columns: new[] { "Id", "BeekeeperId", "Description", "ImageUrl", "NetWeight", "Price", "Title" },
                values: new object[] { new Guid("a1522dbb-5d38-4345-b816-19ef380ab57e"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), "Asen Asenev's rich in vitamins and minerals bee pollen. It has the riches of bulgarian nature", "https://naturalvita.co.uk/wp-content/uploads/2018/03/bee-pollen-natruralvita-1024x1024.jpg", 100, 9.00m, "Asen's Bee Pollen" });

            migrationBuilder.InsertData(
                table: "Honeys",
                columns: new[] { "Id", "BeekeeperId", "CategoryId", "Description", "ImageUrl", "NetWeight", "Origin", "Price", "Title", "YearMade" },
                values: new object[] { new Guid("eb344c41-6baf-40d3-8daa-897ca2d945b2"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), 1, "High quality linden honey from Asen Asenev.", "https://beehoneyportal.com/wp-content/uploads/2014/10/burkan-s-med-3.jpg", 450, "Vratsa", 12.00m, "Asen's Linden Honey", 2022 });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "IsActive", "Title", "UserId" },
                values: new object[] { 1, "Welcome to this new site, I am the first beekeeper hereenjoy your stay.", true, "The site's first post", new Guid("29a205b5-19c1-4dbb-a318-0235f51af7c7") });

            migrationBuilder.InsertData(
                table: "Propolises",
                columns: new[] { "Id", "BeekeeperId", "Description", "FlavourId", "ImageUrl", "Price", "Title" },
                values: new object[] { new Guid("b49b7061-ad27-42cd-b46d-56b528499dae"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), "30% tincture that is good for the immune system with anti-inflammatory properties.", 1, "https://www.apihealth.co.nz/wp-content/uploads/2019/07/Propolis-Tincture.jpg", 3.00m, "Bee Propolis" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DeleteData(
                table: "BeePollens",
                keyColumn: "Id",
                keyValue: new Guid("a1522dbb-5d38-4345-b816-19ef380ab57e"));

            migrationBuilder.DeleteData(
                table: "Honeys",
                keyColumn: "Id",
                keyValue: new Guid("eb344c41-6baf-40d3-8daa-897ca2d945b2"));

            migrationBuilder.DeleteData(
                table: "Propolises",
                keyColumn: "Id",
                keyValue: new Guid("b49b7061-ad27-42cd-b46d-56b528499dae"));

            migrationBuilder.InsertData(
                table: "BeePollens",
                columns: new[] { "Id", "BeekeeperId", "CreatedOn", "Description", "ImageUrl", "IsActive", "NetWeight", "Price", "Title" },
                values: new object[] { new Guid("e08d70b3-0f2b-4c21-b4f7-46a69713f8a1"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Asen Asenev's rich in vitamins and minerals bee pollen. It has the riches of bulgarian nature", "https://naturalvita.co.uk/wp-content/uploads/2018/03/bee-pollen-natruralvita-1024x1024.jpg", false, 100, 9.00m, "Asen's Bee Pollen" });

            migrationBuilder.InsertData(
                table: "Honeys",
                columns: new[] { "Id", "BeekeeperId", "CategoryId", "CreatedOn", "Description", "ImageUrl", "IsActive", "NetWeight", "Origin", "Price", "Title", "YearMade" },
                values: new object[] { new Guid("5e337878-8e69-48d8-ab74-3df44f357401"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "High quality linden honey from Asen Asenev.", "https://beehoneyportal.com/wp-content/uploads/2014/10/burkan-s-med-3.jpg", false, 450, "Vratsa", 12.00m, "Asen's Linden Honey", 2022 });

            migrationBuilder.InsertData(
                table: "Propolises",
                columns: new[] { "Id", "BeekeeperId", "CreatedOn", "Description", "FlavourId", "ImageUrl", "IsActive", "Price", "Title" },
                values: new object[] { new Guid("5af6b804-167f-4873-8571-aa242022a45c"), new Guid("a38f5e91-11ff-40b2-9987-317c60fec5a5"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "30% tincture that is good for the immune system with anti-inflammatory properties.", 1, "https://www.apihealth.co.nz/wp-content/uploads/2019/07/Propolis-Tincture.jpg", false, 3.00m, "Bee Propolis" });
        }
    }
}
