using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProgRunner.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlphaChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlphaChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RunLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    SubmittedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlphaChallengeTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlphaChallengeId = table.Column<int>(type: "int", nullable: true),
                    TopInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BottomInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expectedOutput = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlphaChallengeTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlphaChallengeTests_AlphaChallenges_AlphaChallengeId",
                        column: x => x.AlphaChallengeId,
                        principalTable: "AlphaChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlphaChallengeTests_AlphaChallengeId",
                table: "AlphaChallengeTests",
                column: "AlphaChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlphaChallengeTests");

            migrationBuilder.DropTable(
                name: "RunLogs");

            migrationBuilder.DropTable(
                name: "AlphaChallenges");
        }
    }
}
