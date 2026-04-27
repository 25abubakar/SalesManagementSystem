using System.ComponentModel.DataAnnotations;

namespace SalesManagementSystem.Models
{
    public class SaleAcctCreateVM
    {
        public int? CompanyId { get; set; }

        [Required(ErrorMessage = "Platform is required.")]
        public int? PlatformId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        [MaxLength(100)]
        public string? OrderID { get; set; }

        public decimal? QtyHeld { get; set; }

        [Required(ErrorMessage = "Qty Sold is required.")]
        public decimal? QtySold { get; set; }

        [Required(ErrorMessage = "Transaction Type is required.")]
        public int? TransactionId { get; set; }

        [Required(ErrorMessage = "From Account is required.")]
        public int? FromAccountId { get; set; }

        [Required(ErrorMessage = "To Account is required.")]
        public int? ToAccountId { get; set; }

        public decimal? TotalProCharges { get; set; }
        public decimal? AmazonFee { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? TotalPromotion { get; set; }

        [Required(ErrorMessage = "Sold Amount is required.")]
        public decimal? SoldAmount { get; set; }

        public decimal? TotalRroRebate { get; set; }

        [Required(ErrorMessage = "Cost Amount is required.")]
        public decimal? CostAmount { get; set; }

        public string? AmzProRef { get; set; }
        public string? Status { get; set; }
        public string? Discription { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int? StatusID { get; set; }

        public string? Action { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<SaleTransactionDateEntryVM> SaleTransactionDates { get; set; } = new();
        public List<SaleChargeEntryVM> Charges { get; set; } = new();
        public long? Id { get; set; }
        public decimal? ProfitAmount { get; set; }
    }

    public class SaleTransactionDateEntryVM
    {
        [Display(Name = "Date Label")]
        public int? DateLabelId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }

    public class SaleChargeEntryVM
    {
        public long? SaleChargeId { get; set; }

        [Display(Name = "Charge Type")]
        public int? ChargeTypeId { get; set; }

        public decimal? Amount { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }
    }
}
