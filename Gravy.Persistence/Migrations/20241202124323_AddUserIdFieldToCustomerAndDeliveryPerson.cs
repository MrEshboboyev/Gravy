using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gravy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdFieldToCustomerAndDeliveryPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Customers_CustomerDetailsId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_DeliveryPersons_DeliveryPersonDetailsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CustomerDetailsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeliveryPersonDetailsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CustomerDetailsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeliveryPersonDetailsId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DeliveryPersons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersons_UserId",
                table: "DeliveryPersons",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryPersons_Users_UserId",
                table: "DeliveryPersons",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryPersons_Users_UserId",
                table: "DeliveryPersons");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryPersons_UserId",
                table: "DeliveryPersons");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DeliveryPersons");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerDetailsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryPersonDetailsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerDetailsId",
                table: "Users",
                column: "CustomerDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeliveryPersonDetailsId",
                table: "Users",
                column: "DeliveryPersonDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Customers_CustomerDetailsId",
                table: "Users",
                column: "CustomerDetailsId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DeliveryPersons_DeliveryPersonDetailsId",
                table: "Users",
                column: "DeliveryPersonDetailsId",
                principalTable: "DeliveryPersons",
                principalColumn: "Id");
        }
    }
}
