using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testingSite.Migrations
{
    /// <inheritdoc />
    public partial class MakeGroupAssignmentNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_GroupAssignments_GroupAssignmentId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "GroupAssignmentId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_GroupAssignments_GroupAssignmentId",
                table: "Assignments",
                column: "GroupAssignmentId",
                principalTable: "GroupAssignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_GroupAssignments_GroupAssignmentId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "GroupAssignmentId",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_GroupAssignments_GroupAssignmentId",
                table: "Assignments",
                column: "GroupAssignmentId",
                principalTable: "GroupAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
