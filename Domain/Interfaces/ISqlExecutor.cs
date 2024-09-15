using System.Data;

namespace Domain.Interfaces;

public interface ISqlExecutor
{
    IEnumerable<T> ExecuteG<T>(string query);
    IEnumerable<T> Query<T>(string query, object param = null, bool buffered = true);
    IEnumerable<IDictionary<string, object>> Query(string query, object param = null);
    IEnumerable<IDictionary<string, string>> Execute(string query, object param = null);
    T ExecuteScalar<T>(string query);
    dynamic QueryFirst(string query);
    dynamic QueryFirst<T>(string query);
    dynamic ExecuteQuery<T>(string query, object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    void ExecuteQuery(string query);
    Task ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    void ExecuteWithTransaction(string sql);
    bool TryValidateSql(string query);
}