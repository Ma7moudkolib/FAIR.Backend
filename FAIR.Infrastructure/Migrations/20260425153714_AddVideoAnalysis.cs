using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FAIR.Infrastructure.Migrations
{
    public partial class AddVideoAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiResultRaw = table.Column<string>(type: "nvarchar(12000)", maxLength: 12000, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoAnalyses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoAnalyses_AthleteId",
                table: "VideoAnalyses",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoAnalyses_CreatedDate",
                table: "VideoAnalyses",
                column: "CreatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoAnalyses");
        }
    }
}
