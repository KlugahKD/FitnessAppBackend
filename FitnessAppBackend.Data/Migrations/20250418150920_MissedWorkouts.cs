using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class MissedWorkouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMissed",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMissed",
                table: "Exercises");
        }
    }
}
