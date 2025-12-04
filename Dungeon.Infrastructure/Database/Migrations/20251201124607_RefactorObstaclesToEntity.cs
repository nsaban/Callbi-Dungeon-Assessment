using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dungeon.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorObstaclesToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Obstacles",
                table: "DungeonMaps");

            migrationBuilder.CreateTable(
                name: "Obstacles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    DungeonMapId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obstacles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obstacles_DungeonMaps_DungeonMapId",
                        column: x => x.DungeonMapId,
                        principalTable: "DungeonMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obstacles_Coordinates_MapId",
                table: "Obstacles",
                columns: new[] { "X", "Y", "DungeonMapId" });

            migrationBuilder.CreateIndex(
                name: "IX_Obstacles_DungeonMapId",
                table: "Obstacles",
                column: "DungeonMapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obstacles");

            migrationBuilder.AddColumn<string>(
                name: "Obstacles",
                table: "DungeonMaps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
