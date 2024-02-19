using Microsoft.Data.SqlClient;
using System.Data;

namespace MoviesDb.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var cnn  = new SqlConnection(_connectionString);
        await cnn.OpenAsync();
        return cnn;
    }
}
