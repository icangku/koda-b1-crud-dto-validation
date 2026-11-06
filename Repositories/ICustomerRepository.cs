using CrudApi.Models;

namespace CrudApi.Repositories;

public interface ICustomerRepository
{
  Task<IReadOnlyList<Customer>> GetAll(CancellationToken cancellationToken);
  Task<Customer?> GetByCode(string id, CancellationToken cancellationToken);
  Task<bool> Add(Customer customer, CancellationToken cancellationToken);
  Task<bool> Update(Customer customer, CancellationToken cancellationToken);
  Task<bool> Delete(string id, CancellationToken cancellationToken);
}