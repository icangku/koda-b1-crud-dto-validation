using System.Data;
using CrudApi.Models;
using Microsoft.Data.SqlClient;

namespace CrudApi.Repositories;

public sealed class ProductRepository : IProductRepository
{
  private readonly string _connectionString;
  public ProductRepository(IConfiguration configuration)
  {
    _connectionString = configuration.GetConnectionString("ecommercedb") ?? throw new InvalidOperationException("Connection lost");
  }

  public async Task<bool> Add(Product product, CancellationToken cancellationToken)
  {
    const string sql = """
        INSERT INTO dbo.Product(Code, Name, Email, Phone, Address) VALUES
        (@code, @name, @email, @phone, @address);
        """;
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);

        AddProductParameter(command, product);

        try
        {
            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

            return affectedRows == 1;
        }
        catch (SqlException e) when (e.Number is 2601 or 2627)
        {
            return false;
        }
        catch (SqlException e)
        {
            Console.WriteLine($"SQL Error tidak terduga: {e.Number} - {e.Message}");
            return false;
        }
  }

  public async Task<bool> Delete(string code, CancellationToken cancellationToken)
  {
    const string sql = """
      DELETE FROM dbo.Product
      WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    AddProductCode(command, code);
    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    return affectedRows == 1;
  }

  public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Code, Name, Price, Stock, Category
      FROM dbo.Product
      ORDER BY Code DESC;
    """;

    var products = new List<Product>();
    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
    while(await reader.ReadAsync(cancellationToken))
    {
      products.Add(ReadProduct(reader));
    }
    return products;
  }

  public async Task<Product?> GetByCode(string code, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Code, Name, Price, Stock, Category FROM dbo.Product WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);

    await connection.OpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, connection);

    AddProductCode(command, code);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
        return null;
    }
    return ReadProduct(reader);
  }

  public async Task<bool> Update(Product product, CancellationToken cancellationToken)
  {
    const string sql = """
        UPDATE dbo.Product SET 
            Name = @name,
            Price = @price,
            Stock = @stock,
            Category = @category
    WHERE Code = @code;
    """;

    await using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(cancellationToken);
    await using var command = new SqlCommand(sql, connection);
    AddProductParameter(command, product);
    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }


  private static void AddProductParameter(SqlCommand command, Product product)
  {
    AddProductCode(command, product.Code);
    command.Parameters.Add("@name", SqlDbType.NVarChar, 120).Value = product.Name;
    command.Parameters.Add("@stock", SqlDbType.Int).Value = product.Stock;
    var price = command.Parameters.Add("@price", SqlDbType.Decimal);
    command.Parameters.Add("@category", SqlDbType.NVarChar, 80).Value = product.Category;

    price.Precision = 18;
    price.Scale = 2;
    price.Value = product.Price;
  }

  private static void AddProductCode(SqlCommand command, string productCode)
  {
    command.Parameters.Add("@code", SqlDbType.NVarChar, 30).Value = productCode.Trim().ToUpperInvariant();
  }

  private static Product ReadProduct(SqlDataReader reader)
  {
    return new Product(
      reader.GetString(0),
      reader.GetString(1),
      reader.GetDecimal(2),
      reader.GetInt32(3),
      reader.GetString(4)
    )
    {
      Code = reader.GetString(0),
      Name = reader.GetString(1),
      Price = reader.GetDecimal(2),
      Stock = reader.GetInt32(3),
      Category = reader.GetString(4)
    };
  }
}