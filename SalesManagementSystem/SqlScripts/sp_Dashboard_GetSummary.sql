CREATE OR ALTER PROCEDURE dbo.sp_Dashboard_GetSummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS TotalSales,
        ISNULL(SUM(ISNULL(SoldAmount, 0)), 0) AS TotalSaleAmount,
        ISNULL(SUM(ISNULL(ProfitAmount, 0)), 0) AS TotalProfitAmount,
        ISNULL(AVG(CONVERT(decimal(18, 2), ISNULL(ProfitAmount, 0))), 0) AS AverageProfitAmount
    FROM dbo.SaleAcct;

    SELECT
        CONCAT(YEAR(CreatedDate), '-', RIGHT('00' + CAST(MONTH(CreatedDate) AS varchar(2)), 2)) AS [Label],
        ISNULL(SUM(ISNULL(SoldAmount, 0)), 0) AS SoldAmount,
        ISNULL(SUM(ISNULL(ProfitAmount, 0)), 0) AS ProfitAmount
    FROM dbo.SaleAcct
    WHERE CreatedDate IS NOT NULL
    GROUP BY YEAR(CreatedDate), MONTH(CreatedDate)
    ORDER BY YEAR(CreatedDate), MONTH(CreatedDate);
END;
GO

CREATE UNIQUE INDEX IX_SaleAcct_Platform_Product_OrderID
ON dbo.SaleAcct (PlatformId, ProductId, OrderID)
WHERE OrderID IS NOT NULL;
GO
