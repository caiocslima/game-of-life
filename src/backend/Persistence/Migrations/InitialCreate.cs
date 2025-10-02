using System;
using GameOfLife.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

[DbContext(typeof(GameDbContext))]
[Migration("20250929120000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Boards",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                StateJson = table.Column<string>(nullable: false),
                Generation = table.Column<int>(nullable: false),
                Stability = table.Column<string>(nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Boards", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Boards");
    }
}