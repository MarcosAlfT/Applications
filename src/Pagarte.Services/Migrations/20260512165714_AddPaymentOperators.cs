using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pagarte.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentOperators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperatorProvider",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "QuoteId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorProvider",
                table: "CreditCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PaymentOperators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOperators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentQuotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentQuotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentQuotes_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentQuoteDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentQuoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentQuoteDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentQuoteDetails_PaymentQuotes_PaymentQuoteId",
                        column: x => x.PaymentQuoteId,
                        principalTable: "PaymentQuotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_QuoteId",
                table: "Payments",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOperators_Code",
                table: "PaymentOperators",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOperators_Scope_IsActive_Priority",
                table: "PaymentOperators",
                columns: new[] { "Scope", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuoteDetails_PaymentQuoteId",
                table: "PaymentQuoteDetails",
                column: "PaymentQuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotes_ClientId",
                table: "PaymentQuotes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotes_ExpiresAt",
                table: "PaymentQuotes",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotes_ServiceId",
                table: "PaymentQuotes",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotes_Status",
                table: "PaymentQuotes",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentQuotes_QuoteId",
                table: "Payments",
                column: "QuoteId",
                principalTable: "PaymentQuotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentQuotes_QuoteId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentOperators");

            migrationBuilder.DropTable(
                name: "PaymentQuoteDetails");

            migrationBuilder.DropTable(
                name: "PaymentQuotes");

            migrationBuilder.DropIndex(
                name: "IX_Payments_QuoteId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OperatorProvider",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OperatorProvider",
                table: "CreditCards");
        }
    }
}
