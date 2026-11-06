using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs;

public record CreateProductRequest(
  [Required, StringLength(30)]
  string Code,

  [Required, StringLength(30)]
  string Name,
  
  [Required, Range(typeof(decimal), "0", "99999999")]
  decimal Price, 
  
  [Range(0, int.MaxValue)]
  int Stock,
  
  [Required, StringLength(100)]
  string Category,

  [Required, Range(1, 9999999999)]
  int CustomerId
);