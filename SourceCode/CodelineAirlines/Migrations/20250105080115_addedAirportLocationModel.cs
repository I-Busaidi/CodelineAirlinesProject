using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodelineAirlines.Migrations
{
    /// <inheritdoc />
    public partial class addedAirportLocationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirportLocations",
                columns: table => new
                {
                    AirportId = table.Column<int>(type: "int", nullable: false),
                    AirportLatitude = table.Column<double>(type: "float", nullable: false),
                    AirportLongitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportLocations", x => new { x.AirportId, x.AirportLatitude, x.AirportLongitude });
                    table.ForeignKey(
                        name: "FK_AirportLocations_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirportLocations");
        }
    }
}
