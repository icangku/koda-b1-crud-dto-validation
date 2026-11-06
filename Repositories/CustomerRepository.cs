using System.Data;
using CrudApi.Models;
using Microsoft.Data.SqlClient;

namespace CrudApi.Repositories;

public class CustomerRepository(IConfiguration configuration) : ICustomerRepository
{
  private readonly string _connectionString = configuration.GetConnectionString("ecommercedb") ?? throw new InvalidOperationException("Connection lost");

  public async Task<bool> Add(Customer customer, CancellationToken cancellationToken)
  {
    const string sql = """
      INSERT INTO dbo.Customer (Code, Name, Email, Phone, Address) 
      VALUES (@code, @name, @email, @phone, @address) 
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    
    AddCustomerParameter(command, customer);
    try
    {
      var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
      return affectedRows == 1;
    }
    catch(SqlException e)
    {
      Console.WriteLine($"Sql Error {e.Number} - {e.Message}");
      return false;
    }
  }

  public async Task<bool> Delete(string code, CancellationToken cancellationToken)
  {
    const string sql = """
      DELETE FROM dbo.Customer
      WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    AddCustomerId(command, code);
    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    return affectedRows == 1;
  }

  public async Task<IReadOnlyList<Customer>> GetAll(CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Code, Name, Email, Phone, Address
      FROM dbo.Customer
      ORDER BY Code DESC;
    """;

    var customers = new List<Customer>();
    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
    while(await reader.ReadAsync(cancellationToken))
    {
      customers.Add(ReadCustomer(reader));
    }
    return customers;
  }

  public async Task<Customer?> GetByCode(string code, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Code, Name, Email, Phone, Address FROM dbo.Customer WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    AddCustomerId(command, code);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
    
    if (!await reader.ReadAsync(cancellationToken))
    {
        return null;
    }
    
    return ReadCustomer(reader);
  }

  public async Task<bool> Update(Customer customer, CancellationToken cancellationToken)
  {
    const string sql = """
        UPDATE dbo.Customer SET 
            Name = @name,
            Email = @email,
            Phone = @phone,
            Address = @address
    WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    AddCustomerParameter(command, customer);
    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  private static void AddCustomerParameter(SqlCommand sqlCommand, Customer customer)
  {
    AddCustomerId(sqlCommand, customer.Code);
    sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = customer.Name;
    sqlCommand.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = customer.Email;
    sqlCommand.Parameters.Add("@phone", SqlDbType.NVarChar, 15).Value = customer.Phone;
    sqlCommand.Parameters.Add("@address", SqlDbType.NVarChar, 200).Value = customer.Address;
  }

  private static void AddCustomerId(SqlCommand sqlCommand, string code)
  {
    sqlCommand.Parameters.Add("@code", SqlDbType.NVarChar, 30).Value = code.Trim();
  }

  private static Customer ReadCustomer(SqlDataReader reader)
  {
    return new Customer(
      reader.GetString(0),
      reader.GetString(1),
      reader.GetString(2),
      reader.GetString(3),
      reader.GetString(4)
    );
  }
}