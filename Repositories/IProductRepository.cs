using CrudApi.Models;

namespace CrudApi.Repositories;


public interface IProductRepository
{
  Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken);
  Task<Product?> GetByCode(string code, CancellationToken cancellationToken);
  Task<bool> Add(Product product, CancellationToken cancellationToken);
  Task<bool> Update(Product product, CancellationToken cancellationToken);
  Task<bool> Delete(string code, CancellationToken cancellationToken);
}