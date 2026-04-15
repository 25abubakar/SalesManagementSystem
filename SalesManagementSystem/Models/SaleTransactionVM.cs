using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesManagementSystem.Models
{
    public class SaleTransactionVM
    {
        public int SaleDateId { get; set; }
        public long SaleAcctId { get; set; }
        public DateTime Date { get; set; }

        public List<SelectListItem>? SaleDateList { get; set; }
        public List<SelectListItem>? SaleAcctList { get; set; }
    }
}