using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementSystem.Models
{
    public class SaleTransactionDate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sale Account")]
        public long SaleAcctId { get; set; }

        [ForeignKey("SaleAcctId")]
        public SaleAcct? SaleAcct { get; set; }

        [Required]
        [Display(Name = "Date Label")]
        public int DateLabelId { get; set; }

        [ForeignKey("DateLabelId")]
        public SaleDate? SaleDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}