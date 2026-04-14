namespace SalesManagementSystem.Models
{
    public class SaleCharge
    {
        public long SaleChargeId { get; set; }

        public long SaleId { get; set; }
        public SaleAcct? Sale { get; set; }

        public int ChargeTypeId { get; set; }

        public decimal Amount { get; set; }
    }
}
