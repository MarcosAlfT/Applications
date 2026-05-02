using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pagarte.Worker.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardNumberAndCvv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "CreditCards",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cvv",
                table: "CreditCards",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "Cvv",
                table: "CreditCards");
        }
    }
}
