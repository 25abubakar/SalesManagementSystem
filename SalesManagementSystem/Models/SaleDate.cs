using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    public class SaleDate
    {
        public DateTime? TransDate { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? SoldDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
