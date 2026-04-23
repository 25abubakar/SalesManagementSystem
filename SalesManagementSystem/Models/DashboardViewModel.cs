using System.Collections.Generic;

namespace SalesManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalSales { get; set; }
        public decimal TotalSoldAmount { get; set; }
        public decimal TotalProfitAmount { get; set; }
        public decimal AverageProfitAmount { get; set; }
        public List<DashboardChartPoint> MonthlySales { get; set; } = new();
        public List<SaleAcct> RecentSales { get; set; } = new();
    }

    public class DashboardChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal SoldAmount { get; set; }
        public decimal ProfitAmount { get; set; }
    }
}
