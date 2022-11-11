using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlisTimer.Migrations
{
    public partial class CleanedEmployeeCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Projects_ProjectId1",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Activities_ActivityId",
                table: "TimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Projects_ProjectId",
                table: "TimeLogs");

            migrationBuilder.RenameColumn(
                name: "ProjectId1",
                table: "Activities",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ProjectId1",
                table: "Activities",
                newName: "IX_Activities_ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Employee",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Activities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Projects_ProjectId",
                table: "Activities",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Activities_ActivityId",
                table: "TimeLogs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Projects_ProjectId",
                table: "TimeLogs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Projects_ProjectId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Activities_ActivityId",
                table: "TimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Projects_ProjectId",
                table: "TimeLogs");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Activities",
                newName: "ProjectId1");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ProjectId",
                table: "Activities",
                newName: "IX_Activities_ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Projects_ProjectId1",
                table: "Activities",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Activities_ActivityId",
                table: "TimeLogs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Projects_ProjectId",
                table: "TimeLogs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
