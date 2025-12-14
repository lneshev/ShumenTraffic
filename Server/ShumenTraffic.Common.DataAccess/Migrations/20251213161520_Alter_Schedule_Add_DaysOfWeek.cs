using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Schedule_Add_DaysOfWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayType",
                table: "Schedules",
                newName: "DaysOfWeek");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DaysOfWeek",
                table: "Schedules",
                column: "DaysOfWeek");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_DaysOfWeek",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "DaysOfWeek",
                table: "Schedules",
                newName: "DayType");
        }
    }
}
