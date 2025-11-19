using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_BusStops_Add_Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "BusStops");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "BusStops");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "BusStops",
                type: "geography",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "BusStops");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "BusStops",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "BusStops",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
