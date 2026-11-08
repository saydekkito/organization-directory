using Microsoft.Data.Sqlite;

namespace organization_directory.Data;

public class DatabaseContext {
    private readonly string _connectionString;

    public DatabaseContext() {
        string databasePath = Path.Combine(
            AppContext.BaseDirectory,
            "organization-directory.db"
        );

        _connectionString = $"Data Source={databasePath}";
    }

    public SqliteConnection CreateConnection() {
        return new SqliteConnection(_connectionString);
    }

    public void InitializeDatabase() {
        using var connection = CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Organizations
            (
              Id INTEGER PRIMARY KEY AUTOINCREMENT,
              Name TEXT NOT NULL,
              INN TEXT NOT NULL UNIQUE,
              Email TEXT NOT NULL,
              ReceptionPhone TEXT NOT NULL,
              HRPhone TEXT NOT NULL,
              AccountingPhone TEXT NOT NULL
            );
            """;

        command.ExecuteNonQuery();
    }
}