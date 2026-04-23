using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelForPivotView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaleAcct_PlatformId",
                table: "SaleAcct");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_PlatformId_ProductId_OrderID",
                table: "SaleAcct",
                columns: new[] { "PlatformId", "ProductId", "OrderID" },
                unique: true,
                filter: "[OrderID] IS NOT NULL");

            migrationBuilder.Sql(
                """
                CREATE VIEW dbo.vwSaleTransactionDatesPivot
                AS
                SELECT
                    pvt.SaleAcctId,
                    [1] AS TransactionDate,
                    [2] AS PaymentDate,
                    [3] AS OrderDate,
                    [4] AS ProcessDate,
                    [5] AS SoldDate
                FROM
                (
                    SELECT
                        SaleAcctId,
                        DateLabelId,
                        [Date]
                    FROM dbo.SaleTransactionDates
                ) AS src
                PIVOT
                (
                    MAX([Date])
                    FOR DateLabelId IN ([1],[2],[3],[4],[5])
                ) AS pvt;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID('dbo.vwSaleTransactionDatesPivot', 'V') IS NOT NULL
                    DROP VIEW dbo.vwSaleTransactionDatesPivot;
                """);

            migrationBuilder.DropIndex(
                name: "IX_SaleAcct_PlatformId_ProductId_OrderID",
                table: "SaleAcct");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_PlatformId",
                table: "SaleAcct",
                column: "PlatformId");
        }
    }
}
