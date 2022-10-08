using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlisTimer.Migrations
{
    public partial class connectionsBetweenTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "Projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Projects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ActivityName",
                table: "Activities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "Activities",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_ActivityId",
                table: "TimeLogs",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_ProjectId",
                table: "TimeLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProjectId",
                table: "Activities",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Projects_ProjectId",
                table: "Activities",
                column: "ProjectId",
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

            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_ActivityId",
                table: "TimeLogs");

            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_ProjectId",
                table: "TimeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Activities_ProjectId",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Projects",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Projects",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Activities",
                newName: "ActivityName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Activities",
                newName: "ActivityId");
        }
    }
}
