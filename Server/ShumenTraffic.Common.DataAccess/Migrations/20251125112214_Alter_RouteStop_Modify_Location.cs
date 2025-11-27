using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_RouteStop_Modify_Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "RouteStops",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "RouteStops",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops",
                column: "RouteId");
        }
    }
}
