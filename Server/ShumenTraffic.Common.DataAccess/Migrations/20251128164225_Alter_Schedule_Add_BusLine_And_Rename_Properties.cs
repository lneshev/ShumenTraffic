using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Schedule_Add_BusLine_And_Rename_Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "Schedules",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                table: "Schedules",
                newName: "StartDate");

            migrationBuilder.AlterColumn<int>(
                name: "DayType",
                table: "Schedules",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "BusLineId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BusLineId",
                table: "Schedules",
                column: "BusLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_BusLines_BusLineId",
                table: "Schedules",
                column: "BusLineId",
                principalTable: "BusLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_BusLines_BusLineId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BusLineId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BusLineId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Schedules",
                newName: "EffectiveDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Schedules",
                newName: "ExpiryDate");

            migrationBuilder.AlterColumn<string>(
                name: "DayType",
                table: "Schedules",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
