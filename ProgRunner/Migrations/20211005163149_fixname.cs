using Microsoft.EntityFrameworkCore.Migrations;

namespace ProgRunner.Migrations
{
    public partial class fixname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "expectedOutput",
                table: "AlphaChallengeTests",
                newName: "ExpectedOutput");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedOutput",
                table: "AlphaChallengeTests",
                newName: "expectedOutput");
        }
    }
}
