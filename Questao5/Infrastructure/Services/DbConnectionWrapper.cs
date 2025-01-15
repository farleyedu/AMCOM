using System.Data;
using Dapper;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Services
{
    public class DbConnectionWrapper : IDbConnectionWrapper
    {
        private readonly IDbConnection _dbConnection;

        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            return _dbConnection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public Task<int> ExecuteAsync(string sql, object param = null)
        {
            return _dbConnection.ExecuteAsync(sql, param);
        }
    }
}
