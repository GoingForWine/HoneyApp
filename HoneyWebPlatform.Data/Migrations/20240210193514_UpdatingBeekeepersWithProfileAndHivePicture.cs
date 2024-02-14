using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyWebPlatform.Data.Migrations
{
    public partial class UpdatingBeekeepersWithProfileAndHivePicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HiveFarmPicturePaths",
                table: "Beekeepers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Beekeepers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiveFarmPicturePaths",
                table: "Beekeepers");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Beekeepers");
        }
    }
}
