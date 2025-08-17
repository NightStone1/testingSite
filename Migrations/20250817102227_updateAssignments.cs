using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testingSite.Migrations
{
    /// <inheritdoc />
    public partial class updateAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LectureId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LectureId",
                table: "Assignments",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TestId",
                table: "Assignments",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Lectures_LectureId",
                table: "Assignments",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Tests_TestId",
                table: "Assignments",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Lectures_LectureId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tests_TestId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_LectureId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TestId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "LectureId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "Assignments");
        }
    }
}
