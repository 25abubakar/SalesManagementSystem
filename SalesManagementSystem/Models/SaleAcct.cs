using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleAcct")]
    public class SaleAcct
    {
        [Key]
        public long Id { get; set; }

        public int? CompanyId { get; set; }
        public int? PlatformId { get; set; }
        public int? ProductId { get; set; }
        [MaxLength(100)]
        public string? OrderID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? QtyHeld { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? QtySold { get; set; }

        public int? TransactionId { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalProCharges { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmazonFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OtherCharges { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPromotion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoldAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalRroRebate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostAmount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ProfitAmount { get; set; }

        [MaxLength(200)]
        public string? AmzProRef { get; set; }

        [MaxLength(500)]
        public string? Status { get; set; }

        public string? Discription { get; set; }
        public int? StatusID { get; set; }

        [MaxLength(100)]
        public string? Action { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual SalePlatform? Platform { get; set; }
        public virtual SaleProduct? Product { get; set; }
        public virtual SaleTransactionType? TransactionType { get; set; }
        public virtual SaleAccount? FromAccount { get; set; }
        public virtual SaleAccount? ToAccount { get; set; }
        public virtual SaleStatus? StatusMaster { get; set; }
        public virtual List<SaleCharge> Charges { get; set; } = new();

        public ICollection<SaleTransactionDate>? SaleTransactionDates { get; set; }

        [NotMapped]
        public string? ChargeTypesCsv { get; set; }

        [NotMapped]
        public string? TransactionDatesCsv { get; set; }
    }
}