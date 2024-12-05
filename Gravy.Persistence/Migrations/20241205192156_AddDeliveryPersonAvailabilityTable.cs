using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gravy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPersonAvailabilityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "DeliveryPersons",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "DeliveryPersonAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPersonAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPersonAvailabilities_DeliveryPersons_DeliveryPerson~",
                        column: x => x.DeliveryPersonId,
                        principalTable: "DeliveryPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersonAvailabilities_DeliveryPersonId",
                table: "DeliveryPersonAvailabilities",
                column: "DeliveryPersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryPersonAvailabilities");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "DeliveryPersons");
        }
    }
}
