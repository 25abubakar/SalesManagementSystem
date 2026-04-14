using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleProduct")]
    public class SaleProduct
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = "";

        public int? PlatformId { get; set; }
        public SalePlatform? Platform { get; set; }

        public int? CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
