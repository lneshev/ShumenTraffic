using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Route_Modify_Unique_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_Name",
                table: "Routes");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Name_BusLineId_Direction",
                table: "Routes",
                columns: new[] { "Name", "BusLineId", "Direction" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_Name_BusLineId_Direction",
                table: "Routes");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Name",
                table: "Routes",
                column: "Name",
                unique: true);
        }
    }
}
