using System.Data;
using Microsoft.Data.SqlClient;

namespace SalesManagementSystem.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DapperContext(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}