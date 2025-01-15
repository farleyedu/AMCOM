namespace Questao5.Domain.Interfaces
{
    public interface IDbConnectionWrapper
    {
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);
        Task<int> ExecuteAsync(string sql, object param = null);

    }
}