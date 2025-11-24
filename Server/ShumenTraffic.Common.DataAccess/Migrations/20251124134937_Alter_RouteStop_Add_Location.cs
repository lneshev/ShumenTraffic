using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_RouteStop_Add_Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "RouteStops");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "RouteStops",
                type: "geography",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "RouteStops");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "RouteStops",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "RouteStops",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopOrder" });
        }
    }
}
