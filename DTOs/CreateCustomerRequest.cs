using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs;

public record CreateCustomerRequest(
  [Required, StringLength(30)]
  string Code,

  [Required, StringLength(50)]
  string Name,

  [Required, StringLength(30)]
  string Email,

  [Required, StringLength(13)]
  string Phone,

  [Required, StringLength(200)]
  string Address
);