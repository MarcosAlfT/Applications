using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pagarte.Worker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreditCardCvv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cvv",
                table: "CreditCards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cvv",
                table: "CreditCards",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
