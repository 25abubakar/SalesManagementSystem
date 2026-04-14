using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SalePlatform")]
    public class SalePlatform
    {
        [Key]
        public int PlatformId { get; set; }
        [Required]
        [MaxLength(150)]
        public string PlatformName { get; set; } = "";

        public int? CompanyId { get; set; }
        public bool IsActive { get; set; }

        public List<SaleProduct> Products { get; set; } = new();
    }
}
