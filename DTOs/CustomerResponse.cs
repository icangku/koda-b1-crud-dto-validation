using CrudApi.Models;

namespace CrudApi.DTOs;

public sealed record CustomerResponse(string Id, string Name, string Email, string Phone, string Address)
{
  public static CustomerResponse FromModel(Customer customer) => new(customer.Code, customer.Name, customer.Email, customer.Phone, customer.Address);
}