using System.ComponentModel.DataAnnotations;

namespace SalesManagementSystem.Models
{
    public class SaleAcctCreateVM
    {
        public int? CompanyId { get; set; }
        public int? PlatformId { get; set; }
        public int? ProductId { get; set; }
        public string? OrderID { get; set; }
        public decimal? QtyHeld { get; set; }
        public decimal? QtySold { get; set; }
        public int? TransactionId { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public decimal? TotalProCharges { get; set; }
        public decimal? AmazonFee { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? TotalPromotion { get; set; }
        public decimal? SoldAmount { get; set; }
        public decimal? TotalRroRebate { get; set; }
        public decimal? CostAmount { get; set; }
        public string? AmzProRef { get; set; }
        public string? Status { get; set; }
        public string? Discription { get; set; }
        public int? StatusID { get; set; }
        public string? Action { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<SaleTransactionDateEntryVM> SaleTransactionDates { get; set; } = new();
    }

    public class SaleTransactionDateEntryVM
    {
        [Display(Name = "Date Label")]
        public int? DateLabelId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}
