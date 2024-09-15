namespace Infrastructure.Repositories;

public interface IConnectionStringProvider
{
    string GetConnectionString();
}