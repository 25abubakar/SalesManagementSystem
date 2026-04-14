namespace SalesManagementSystem.Models
{
    public class SaleAccount
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }

        public string? AccountType { get; set; }

        public int? PlatformId { get; set; }
        public SalePlatform? Platform { get; set; }

        public bool IsActive { get; set; }
    }
}
