using CrudApi.DTOs;
using CrudApi.Models;

namespace CrudApi.Services;

public interface IProductService
{
  Task<IReadOnlyList<ProductResponse>> GetAll(CancellationToken cancellationToken);
  Task<ProductResponse?> GetByCode(string code, CancellationToken cancellationToken);
  Task<IReadOnlyList<CustomerProductResponse>> GetByCustomerCode(int id, CancellationToken cancellationToken);
  Task<ProductResponse?> Create(CreateProductRequest request, CancellationToken cancellationToken);
  Task<ProductResponse?> Update(string code, UpdateProductRequest updateProductRequest, CancellationToken cancellationToken);
  Task<bool> Delete(string code, CancellationToken cancellationToken);
}