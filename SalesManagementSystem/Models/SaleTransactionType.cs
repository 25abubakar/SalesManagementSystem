using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleTransactionType")]
    public class SaleTransactionType
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string TransactionName { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}
