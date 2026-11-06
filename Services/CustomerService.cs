using CrudApi.DTOs;
using CrudApi.Models;
using CrudApi.Repositories;

namespace CrudApi.Services;

public sealed class CustomerService : ICustomerService
{
  private readonly ICustomerRepository _repository;
  public CustomerService(ICustomerRepository repository)
  {
    _repository = repository;
  }

  public async Task<CustomerResponse?> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
  {
    var customer = new Customer(request.Code, request.Name, request.Email, request.Phone, request.Address);
    var created = await _repository.Add(customer, cancellationToken);
    
    return created ? CustomerResponse.FromModel(customer) : null;
  }

  public Task<bool> Delete(string code, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
    {
      return Task.FromResult(false);
    }

    return _repository.Delete(code, cancellationToken);
  }

  public async Task<IReadOnlyList<CustomerResponse>> GetAll(CancellationToken cancellationToken)
  {
    var customers = await _repository.GetAll(cancellationToken);

    return customers.Select(CustomerResponse.FromModel).ToArray();
  }

  public async Task<CustomerResponse?> GetByCode(string code, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
    {
      return null;
    }

    var customer = await _repository.GetByCode(code, cancellationToken);
    return customer is null ? null : CustomerResponse.FromModel(customer);
  }

  public async Task<CustomerResponse?> Update(string code, UpdateCustomerRequest request, CancellationToken cancellationToken)
  {
    var customer = new Customer(code, request.Name, request.Email, request.Phone, request.Address);
    var updated = await _repository.Update(customer, cancellationToken);

    return updated ? CustomerResponse.FromModel(customer) : null;
  }
}