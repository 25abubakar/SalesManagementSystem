using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenamePivotViewToSaleTransactionDatesPivot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID('dbo.vwSaleTransactionDatesPivot', 'V') IS NOT NULL
                    DROP VIEW dbo.vwSaleTransactionDatesPivot;

                IF OBJECT_ID('dbo.SaleTransactionDatesPivot', 'V') IS NOT NULL
                    DROP VIEW dbo.SaleTransactionDatesPivot;

                EXEC('
                CREATE VIEW dbo.SaleTransactionDatesPivot
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
                ');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID('dbo.SaleTransactionDatesPivot', 'V') IS NOT NULL
                    DROP VIEW dbo.SaleTransactionDatesPivot;

                EXEC('
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
                ');
                """);
        }
    }
}
