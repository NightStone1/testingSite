using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testingSite.Migrations
{
    /// <inheritdoc />
    public partial class updateGroupAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "GroupAssignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssignments_TestId",
                table: "GroupAssignments",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssignments_Tests_TestId",
                table: "GroupAssignments",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssignments_Tests_TestId",
                table: "GroupAssignments");

            migrationBuilder.DropIndex(
                name: "IX_GroupAssignments_TestId",
                table: "GroupAssignments");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "GroupAssignments");
        }
    }
}
