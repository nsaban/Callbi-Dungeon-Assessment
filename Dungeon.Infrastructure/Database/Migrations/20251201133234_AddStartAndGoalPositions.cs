using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dungeon.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStartAndGoalPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoalX",
                table: "DungeonMaps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalY",
                table: "DungeonMaps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartX",
                table: "DungeonMaps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartY",
                table: "DungeonMaps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalX",
                table: "DungeonMaps");

            migrationBuilder.DropColumn(
                name: "GoalY",
                table: "DungeonMaps");

            migrationBuilder.DropColumn(
                name: "StartX",
                table: "DungeonMaps");

            migrationBuilder.DropColumn(
                name: "StartY",
                table: "DungeonMaps");
        }
    }
}
