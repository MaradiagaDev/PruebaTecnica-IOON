using System.Data;
using System.Data.SqlClient;

namespace GestionComercioIOON.Services
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection GetConnection()
        {
            _connection = new SqlConnection(_connectionString);
            return _connection;
        }

        public IDbCommand CreateCommand(string storedProcedureName, CommandType commandType = CommandType.StoredProcedure)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = storedProcedureName;
            return command;
        }


        public void OpenConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

    }
}
