using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleChargeType")]
    public class SaleChargeType
    {
        [Key]
        public int ChargeTypeId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ChargeTypeName { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}
