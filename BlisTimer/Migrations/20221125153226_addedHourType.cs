using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlisTimer.Migrations
{
    public partial class addedHourType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HourTypeId",
                table: "TimeLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HourType",
                columns: table => new
                {
                    HourTypeId = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    WorkActivityId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourType", x => x.HourTypeId);
                    table.ForeignKey(
                        name: "FK_HourType_WorkActivities_WorkActivityId",
                        column: x => x.WorkActivityId,
                        principalTable: "WorkActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_HourTypeId",
                table: "TimeLogs",
                column: "HourTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HourType_WorkActivityId",
                table: "HourType",
                column: "WorkActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_HourType_HourTypeId",
                table: "TimeLogs",
                column: "HourTypeId",
                principalTable: "HourType",
                principalColumn: "HourTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_HourType_HourTypeId",
                table: "TimeLogs");

            migrationBuilder.DropTable(
                name: "HourType");

            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_HourTypeId",
                table: "TimeLogs");

            migrationBuilder.DropColumn(
                name: "HourTypeId",
                table: "TimeLogs");
        }
    }
}
