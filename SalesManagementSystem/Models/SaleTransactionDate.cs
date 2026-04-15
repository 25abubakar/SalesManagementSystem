using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleTransactionDate")]
    public class SaleTransactionDate
    {
        [Key]
        public int SaleTransId { get; set; }

        public int SaleDateId { get; set; }
        public long SaleAcctId { get; set; }

        public DateTime? Date { get; set; }

        [ForeignKey("SaleDateId")]
        public SaleDate SaleDate { get; set; }

        [ForeignKey("SaleAcctId")]
        public SaleAcct SaleAcct { get; set; }
    }
}