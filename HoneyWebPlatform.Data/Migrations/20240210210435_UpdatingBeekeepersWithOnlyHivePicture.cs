using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class UpdatingBeekeepersWithOnlyHivePicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Beekeepers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Beekeepers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
