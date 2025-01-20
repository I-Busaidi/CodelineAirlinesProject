using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodelineAirlines.Migrations
{
    /// <inheritdoc />
    public partial class AddedAirplaneSpecsModelAndSeatLocationInSeatTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWindowSeat",
                table: "SeatTemplates");

            migrationBuilder.AddColumn<int>(
                name: "SeatLocation",
                table: "SeatTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "AirplaneModel",
                table: "Airplanes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AirplaneSpecs",
                columns: table => new
                {
                    Model = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AvgSpeed = table.Column<double>(type: "float", nullable: false),
                    PassengerCapacity = table.Column<int>(type: "int", nullable: false),
                    LuggageCapacity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirplaneSpecs", x => x.Model);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airplanes_AirplaneModel",
                table: "Airplanes",
                column: "AirplaneModel");

            migrationBuilder.AddForeignKey(
                name: "FK_Airplanes_AirplaneSpecs_AirplaneModel",
                table: "Airplanes",
                column: "AirplaneModel",
                principalTable: "AirplaneSpecs",
                principalColumn: "Model",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeatTemplates_AirplaneSpecs_AirplaneModel",
                table: "SeatTemplates",
                column: "AirplaneModel",
                principalTable: "AirplaneSpecs",
                principalColumn: "Model",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airplanes_AirplaneSpecs_AirplaneModel",
                table: "Airplanes");

            migrationBuilder.DropForeignKey(
                name: "FK_SeatTemplates_AirplaneSpecs_AirplaneModel",
                table: "SeatTemplates");

            migrationBuilder.DropTable(
                name: "AirplaneSpecs");

            migrationBuilder.DropIndex(
                name: "IX_Airplanes_AirplaneModel",
                table: "Airplanes");

            migrationBuilder.DropColumn(
                name: "SeatLocation",
                table: "SeatTemplates");

            migrationBuilder.AddColumn<bool>(
                name: "IsWindowSeat",
                table: "SeatTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "AirplaneModel",
                table: "Airplanes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
