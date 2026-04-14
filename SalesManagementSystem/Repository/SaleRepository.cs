using Dapper;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;
using System.Data;

namespace SalesManagementSystem.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DapperContext _context;

        public SaleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SaleAcct>> GetAll()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SaleAcct>("sp_Sale_GetAll", commandType: CommandType.StoredProcedure);
        }

        public async Task<SaleAcct> GetById(long id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SaleAcct>(
                       "sp_Sale_GetById",
                       new { Id = id },
                       commandType: CommandType.StoredProcedure)
                   ?? new SaleAcct();
        }

        public async Task Create(SaleAcct saleacct)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("sp_Sale_Insert", saleacct, commandType: CommandType.StoredProcedure);
        }

        public async Task Update(SaleAcct saleacct)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("sp_Sale_Update", saleacct, commandType: CommandType.StoredProcedure);
        }

        public async Task Delete(long id)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("sp_Sale_Delete",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}