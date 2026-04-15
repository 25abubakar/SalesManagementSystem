using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleDate")]
    public class SaleDate
    {
        [Key]
        public int SaleDateId { get; set; }

        public DateTime? TransDate { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? SoldDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public ICollection<SaleTransactionDate>? SaleTransactionDates { get; set; }
    }
}