namespace SalesManagementSystem.Models
{
    public class SaleTransactionDatePivot
    {
        public long SaleAcctId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? SoldDate { get; set; }
    }
}
