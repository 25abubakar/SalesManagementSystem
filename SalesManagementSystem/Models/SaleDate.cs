using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    public class SaleDate
    {
        public int Id { get; set; }
        public string DateLabel { get; set; }

        public ICollection<SaleTransactionDate>? SaleTransactionDates { get; set; }
    }
}