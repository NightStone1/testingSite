using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testingSite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignmentsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupAssignments_TestCategories_TestCategoryId",
                table: "GroupAssignments");

            migrationBuilder.DropIndex(
                name: "IX_GroupAssignments_TestCategoryId",
                table: "GroupAssignments");

            migrationBuilder.DropColumn(
                name: "TestCategoryId",
                table: "GroupAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestCategoryId",
                table: "GroupAssignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssignments_TestCategoryId",
                table: "GroupAssignments",
                column: "TestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAssignments_TestCategories_TestCategoryId",
                table: "GroupAssignments",
                column: "TestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "Id");
        }
    }
}
