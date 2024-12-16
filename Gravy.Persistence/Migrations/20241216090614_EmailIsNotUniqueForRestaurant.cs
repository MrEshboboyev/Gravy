using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gravy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailIsNotUniqueForRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurants_Email",
                table: "Restaurants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_Email",
                table: "Restaurants",
                column: "Email",
                unique: true);
        }
    }
}
