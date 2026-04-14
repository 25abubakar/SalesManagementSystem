using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SalesManagementSystem.Models
{
    [Table("SaleAccount")]
    public class SaleAccount
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
        [MaxLength(150)]
        public string AccountName { get; set; } = "";

        [MaxLength(50)]
        public string? AccountType { get; set; }

        public int? CompanyId { get; set; }
        public int? PlatformId { get; set; }
        public SalePlatform? Platform { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
