namespace SalesManagementSystem.Models
{
    public class SalePlatform
    {
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }

        public bool IsActive { get; set; }

        public List<SaleProduct>? Products { get; set; }
    }
}
