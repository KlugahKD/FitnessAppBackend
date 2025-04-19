using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class Image_And_Description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "HealthAdvice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "HealthAdvice");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exercises");
        }
    }
}
