using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs;

public record class UpdateCustomerRequest(
  [Required, StringLength(50)]
  string Name,

  [Required, StringLength(50)]
  string Email,

  [Required, StringLength(13)]
  string Phone,

  [Required, StringLength(200)]
  string Address
);