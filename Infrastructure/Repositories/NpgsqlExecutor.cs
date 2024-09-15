using System.Data;
using Domain.Interfaces;
using Npgsql;
using Dapper;

namespace Infrastructure.Repositories;

public class NpgsqlExecutor : ISqlExecutor, IDisposable
{
    private NpgsqlConnection _db;

    public NpgsqlExecutor(IConnectionStringProvider connectionStringProvider)
    {
        _db = new NpgsqlConnection(connectionStringProvider.GetConnectionString());
    }

    public IEnumerable<T> ExecuteG<T>(string query)
    {
        return _db.Query<T>(query);
    }

    public IEnumerable<T> Query<T>(string query, object param = null!, bool buffered = true)
    {
        return _db.Query<T>(query, param);
    }

    public IEnumerable<IDictionary<string, object>> Query(string query, object param = null!)
    {
        return (_db.Query(query, param) as IEnumerable<IDictionary<string, object>>)!;
    }

    public IEnumerable<IDictionary<string, string>> Execute(string query, object param = null!)
    {
        var result = Query(query, param);

        if (result == null!)
        {
            return new List<IDictionary<string, string>>();
        }

        var finalList = new List<Dictionary<string, object>>();

        foreach (var item in result)
        {
            var dictionary = new Dictionary<string, object>();
            var k = item.Select(s => s.Key).ToList();
            var index = 1;
            foreach (var innerKey in k)
            {
                if (dictionary.ContainsKey(innerKey))
                {
                    var newKey = $"{innerKey}_{index}";
                    var value = item.LastOrDefault(s => s.Key == innerKey).Value;
                    dictionary.Add(newKey, value);
                }
                else
                {
                    var value = item.FirstOrDefault(s => s.Key == innerKey).Value;
                    dictionary.Add(innerKey, value);
                }

                index++;
            }

            finalList.Add(dictionary);

        }
        var outData = finalList.Select(r => r.ToDictionary(d => d.Key, d => d.Value.ToString()));
        return outData!;
    }

    public T ExecuteScalar<T>(string query)
    {
        return _db.ExecuteScalar<T>(query);
    }

    public dynamic QueryFirst(string query)
    {
        return _db.QueryFirst(query);
    }

    public dynamic QueryFirst<T>(string query)
    {
        return _db.QueryFirst<T>(query)!;
    }

    public dynamic ExecuteQuery<T>(string query, object? param = null, IDbTransaction? transaction = null, bool buffered = true,
        int? commandTimeout = null, CommandType? commandType = null)
    {
        return _db.Query<T>(query, param, transaction, buffered, commandTimeout, commandType);
    }

    public void ExecuteQuery(string query)
    {
        _db.Query(query);
    }

    public async Task ExecuteAsync(string query, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null,
        CommandType? commandType = null)
    {
        await _db.ExecuteAsync(query, param, transaction, commandTimeout, commandType);
    }

    public void ExecuteWithTransaction(string sql)
    {
        _db.Open();

        using var transaction = _db.BeginTransaction();

        try
        {
            _db.Execute(sql, transaction: transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            _db.Close();
        }
    }

    public bool TryValidateSql(string query)
    {
        try
        {
            _db.Open();

            using var transaction = _db.BeginTransaction();
            using var cmd = _db.CreateCommand();

            cmd.Connection = _db;
            cmd.CommandText = query;
            cmd.Transaction = transaction;

            cmd.Prepare(); // Validates the SQL
            cmd.ExecuteNonQuery();
            transaction.Rollback();

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _db.Close();
        }
    }

    public void Dispose()
    {
        if (_db != null!)
        {
            _db.Dispose();
        }
    }
}