using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleCharge")]
    public class SaleCharge
    {
        [Key]
        public long SaleChargeId { get; set; }

        public long SaleId { get; set; }
        public SaleAcct? Sale { get; set; }

        public int ChargeTypeId { get; set; }
        public SaleChargeType? ChargeType { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }
    }
}
