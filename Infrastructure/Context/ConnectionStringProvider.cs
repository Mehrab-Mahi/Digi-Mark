using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Context;

public class ConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration _configuration;

    public ConnectionStringProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        return _configuration.GetConnectionString(Enum.GetName(typeof(DbConnection), DbConnection.DmDbConnection)!)!; 
    }
}