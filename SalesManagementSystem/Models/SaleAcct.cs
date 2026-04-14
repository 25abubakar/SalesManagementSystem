namespace SalesManagementSystem.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SaleAcct
    {
        [Key]
        public long Id { get; set; }

        public int? CompanyId { get; set; }

        public int? PlatformId { get; set; }
        public SalePlatform? Platform { get; set; }

        public int? ProductId { get; set; }
        public SaleProduct? Product { get; set; }

        public string? OrderID { get; set; }

        public decimal? QtyHeld { get; set; }
        public decimal? QtySold { get; set; }

        public int? TransactionId { get; set; }
        public SaleTransactionType? Transaction { get; set; }

        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }

        public decimal? SoldAmount { get; set; }
        public decimal? CostAmount { get; set; }

        public decimal? TotalProCharges { get; set; }
        public decimal? AmazonFee { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? TotalPromotion { get; set; }
        public decimal? TotalRroRebate { get; set; }

        public decimal? ProfitAmount { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
