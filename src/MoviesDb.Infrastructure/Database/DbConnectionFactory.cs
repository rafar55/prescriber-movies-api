using Microsoft.Data.SqlClient;
using System.Data;

namespace MoviesDb.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
    Task<IDbConnection> CreateMasterConnection();
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private readonly string _masterConnection;

    public SqlServerConnectionFactory(string connectionString, string masterConnection)
    {
        _connectionString = connectionString;
        _masterConnection = masterConnection;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var cnn  = new SqlConnection(_connectionString);
        await cnn.OpenAsync();
        return cnn;
    }

    public async Task<IDbConnection> CreateMasterConnection()
    {
        var cnn = new SqlConnection(_masterConnection);
        await cnn.OpenAsync();
        return cnn;
    }
}
