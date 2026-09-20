using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alborz.Infrastructure.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class UniqueDocumentProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseReceiptItems_PurchaseReceiptId",
                table: "PurchaseReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItems_PurchaseReceiptId_ProductId",
                table: "PurchaseReceiptItems",
                columns: new[] { "PurchaseReceiptId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId_ProductId",
                table: "InvoiceItems",
                columns: new[] { "InvoiceId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseReceiptItems_PurchaseReceiptId_ProductId",
                table: "PurchaseReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_InvoiceId_ProductId",
                table: "InvoiceItems");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItems_PurchaseReceiptId",
                table: "PurchaseReceiptItems",
                column: "PurchaseReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");
        }
    }
}
