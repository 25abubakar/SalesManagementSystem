namespace SalesManagementSystem.Repository
{
    using SalesManagementSystem.Models;

    public interface ISaleRepository
    {
        Task<IEnumerable<SaleAcct>> GetAll();
        Task<SaleAcct> GetById(long id);
        Task Create(SaleAcct saleacct);
        Task Update(SaleAcct saleacct);
        Task Delete(long id);
    }
}
