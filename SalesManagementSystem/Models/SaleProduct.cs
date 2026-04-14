namespace SalesManagementSystem.Models
{
    public class SaleProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int? PlatformId { get; set; }
        public SalePlatform? Platform { get; set; }

        public bool IsActive { get; set; }
    }
}
