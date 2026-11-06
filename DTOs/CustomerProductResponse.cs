namespace CrudApi.DTOs;
public sealed class CustomerProductResponse
{
    public string CustomerName { get; init; }
    public string ProductCode { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Category { get; init; }

    public static CustomerProductResponse FromDto(CustomerProductResponse dto)
    {
        return new CustomerProductResponse
        {
            CustomerName = dto.CustomerName,
            ProductCode = dto.ProductCode,
            ProductName = dto.ProductName,
            Price = dto.Price,
            Stock = dto.Stock,
            Category = dto.Category
        };
    }
}