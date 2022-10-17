using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlisTimer.Migrations
{
    public partial class AddedUserTableWithMMConnectionToProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Projects_ProjectId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");

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

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProject",
                columns: table => new
                {
                    EmployeesId = table.Column<string>(type: "text", nullable: false),
                    ProjectsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProject", x => new { x.EmployeesId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_EmployeeProject_Employee_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_ProjectsId",
                table: "EmployeeProject",
                column: "ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Projects_ProjectId1",
                table: "Activities",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Projects_ProjectId1",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "EmployeeProject");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.RenameColumn(
                name: "ProjectId1",
                table: "Activities",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ProjectId1",
                table: "Activities",
                newName: "IX_Activities_ProjectId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
