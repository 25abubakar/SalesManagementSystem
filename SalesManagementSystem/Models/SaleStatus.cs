using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    [Table("SaleStatus")]
    public class SaleStatus
    {
        [Key]
        public int StatusID { get; set; }
        [Required]
        [MaxLength(100)]
        public string StatusName { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}
