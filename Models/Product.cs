
using System.ComponentModel.DataAnnotations;

namespace CrudApi.Models;

public sealed class Product(string code, string name, decimal price, int stock, string category)
{
  [Required, StringLength(30)]
  public string Code { get; init;} = code;
  [Required, StringLength(30)]
  public string Name { get; init; } = name;
  [Range(typeof(decimal), "0", "9999999")]
  public decimal Price { get; init;} = price;
  [Range(0, int.MaxValue)]
  public int Stock { get; init;} = stock;
  [Required, StringLength(50)]
  public string Category { get; init;} = category.Trim();
}