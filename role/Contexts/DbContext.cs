using System.Data;
using MySql.Data.MySqlClient;

namespace role_api.Contexts;

public class DbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public DbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    public IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}