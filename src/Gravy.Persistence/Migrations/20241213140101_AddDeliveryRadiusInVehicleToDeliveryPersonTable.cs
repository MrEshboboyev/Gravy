using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gravy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryRadiusInVehicleToDeliveryPersonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaxDeliveryRadius",
                table: "DeliveryPersons",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDeliveryRadius",
                table: "DeliveryPersons");
        }
    }
}
