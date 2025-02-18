using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gravy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLockedToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Orders");
        }
    }
}
