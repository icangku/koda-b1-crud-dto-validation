using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs;

public record class UpdateProductRequest(
  [Required, StringLength(30)]
  string Name, 
  
  [Range(typeof(decimal), "0", "9999999")]
  decimal Price, 
  
  [Range(0, int.MaxValue)]
  int Stock, 

  [Required, StringLength(30)]
  string Category,

  [Required, Range(0, 999)]
  int CustomerId
);