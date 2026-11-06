using CrudApi.DTOs;
using CrudApi.Models;

namespace CrudApi.Services;

public interface ICustomerService
{
  Task<IReadOnlyList<CustomerResponse>> GetAll(CancellationToken cancellationToken);
  Task<CustomerResponse?> GetByCode(string code, CancellationToken cancellationToken);
  Task<CustomerResponse?> Create(CreateCustomerRequest request, CancellationToken cancellationToken);
  Task<CustomerResponse?> Update(string code, UpdateCustomerRequest request, CancellationToken cancellationToken);
  Task<bool> Delete(string code, CancellationToken cancellationToken);
}