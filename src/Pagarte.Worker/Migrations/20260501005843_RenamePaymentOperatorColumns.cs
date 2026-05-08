using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pagarte.Worker.Migrations
{
    /// <inheritdoc />
    public partial class RenamePaymentOperatorColumns : Migration
    {
		/// <inheritdoc />
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "DLocalCardToken",
				table: "CreditCards",
				newName: "OperatorCardToken");

			migrationBuilder.RenameColumn(
				name: "DLocalPaymentId",
				table: "Payments",
				newName: "OperatorPaymentId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "OperatorCardToken",
				table: "CreditCards",
				newName: "DLocalCardToken");

			migrationBuilder.RenameColumn(
				name: "OperatorPaymentId",
				table: "Payments",
				newName: "DLocalPaymentId");
		}
	}
}
