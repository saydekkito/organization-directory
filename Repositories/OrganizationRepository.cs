using organization_directory.Data;
using organization_directory.Models;

namespace organization_directory.Repositories;

public class OrganizationRepository {
    private readonly DatabaseContext _databaseContext;

    public OrganizationRepository(DatabaseContext databaseContext) {
        _databaseContext = databaseContext;
    }

    public List<Organization> GetAll() {
        var organizations = new List<Organization>();

        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
                              SELECT Id, Name, INN, Email,
                                     ReceptionPhone, HRPhone, AccountingPhone
                              FROM Organizations;
                              """;

        using var reader = command.ExecuteReader();

        while (reader.Read()) {
            organizations.Add(new Organization {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                INN = reader.GetString(2),
                Email = reader.GetString(3),
                ReceptionPhone = reader.GetString(4),
                HRPhone = reader.GetString(5),
                AccountingPhone = reader.GetString(6)
            });
        }

        return organizations;
    }

    public Organization? GetById(int id) {
        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
                              SELECT Id, Name, INN, Email,
                                     ReceptionPhone, HRPhone, AccountingPhone
                              FROM Organizations
                              WHERE Id = @Id;
                              """;

        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read()) {
            return new Organization {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                INN = reader.GetString(2),
                Email = reader.GetString(3),
                ReceptionPhone = reader.GetString(4),
                HRPhone = reader.GetString(5),
                AccountingPhone = reader.GetString(6)
            };
        }

        return null;
    }

    public int Add(Organization organization) {
        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
                              INSERT INTO Organizations
                                  (Name, INN, Email, ReceptionPhone, HRPhone, AccountingPhone)
                              VALUES
                                  (@Name, @INN, @Email, @ReceptionPhone, @HRPhone, @AccountingPhone);
                              """;

        command.Parameters.AddWithValue("@Name", organization.Name);
        command.Parameters.AddWithValue("@INN", organization.INN);
        command.Parameters.AddWithValue("@Email", organization.Email);
        command.Parameters.AddWithValue("@ReceptionPhone", organization.ReceptionPhone);
        command.Parameters.AddWithValue("@HRPhone", organization.HRPhone);
        command.Parameters.AddWithValue("@AccountingPhone", organization.AccountingPhone);

        command.ExecuteNonQuery();

        using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";

        return Convert.ToInt32(idCommand.ExecuteScalar());
    }

    public void AddRange(List<Organization> organizations) {
        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        foreach (var organization in organizations) {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = """
                                  INSERT INTO Organizations
                                      (Name, INN, Email, ReceptionPhone, HRPhone, AccountingPhone)
                                  VALUES
                                      (@Name, @INN, @Email, @ReceptionPhone, @HRPhone, @AccountingPhone);
                                  """;

            command.Parameters.AddWithValue("@Name", organization.Name);
            command.Parameters.AddWithValue("@INN", organization.INN);
            command.Parameters.AddWithValue("@Email", organization.Email);
            command.Parameters.AddWithValue("@ReceptionPhone", organization.ReceptionPhone);
            command.Parameters.AddWithValue("@HRPhone", organization.HRPhone);
            command.Parameters.AddWithValue("@AccountingPhone", organization.AccountingPhone);

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public bool Update(Organization organization) {
        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
                              UPDATE Organizations
                              SET
                                  Name = @Name,
                                  INN = @INN,
                                  Email = @Email,
                                  ReceptionPhone = @ReceptionPhone,
                                  HRPhone = @HRPhone,
                                  AccountingPhone = @AccountingPhone
                              WHERE Id = @Id;
                              """;

        command.Parameters.AddWithValue("@Id", organization.Id);
        command.Parameters.AddWithValue("@Name", organization.Name);
        command.Parameters.AddWithValue("@INN", organization.INN);
        command.Parameters.AddWithValue("@Email", organization.Email);
        command.Parameters.AddWithValue("@ReceptionPhone", organization.ReceptionPhone);
        command.Parameters.AddWithValue("@HRPhone", organization.HRPhone);
        command.Parameters.AddWithValue("@AccountingPhone", organization.AccountingPhone);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id) {
        using var connection = _databaseContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
                              DELETE FROM Organizations
                              WHERE Id = @Id;
                              """;

        command.Parameters.AddWithValue("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }
}