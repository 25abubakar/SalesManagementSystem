namespace SalesManagementSystem.Models
{
    public class SaleTransactionType
    {
        public int TransactionId { get; set; }
        public string TransactionName { get; set; }

        public bool IsActive { get; set; }
    }
}
