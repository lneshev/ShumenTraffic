using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Alter_BusLine_Add_LineNumberSortKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineNumberSortKey",
                table: "BusLines",
                type: "nvarchar(63)",
                maxLength: 63,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineNumberSortKey",
                table: "BusLines");
        }
    }
}
