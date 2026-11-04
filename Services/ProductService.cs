using CrudApi.DTOs;
using CrudApi.Models;
using CrudApi.Repositories;

namespace CrudApi.Services;

public sealed class ProductService : IProductService
{
  private readonly IProductRepository _repository;
  public ProductService(IProductRepository repository)
  {
    _repository = repository;
  }

  public async Task<ProductResponse?> Create(CreateProductRequest request, CancellationToken cancellationToken)
  {
    var product = new Product(request.Code, request.Name, request.Price, request.Stock, request.Category);
    var created = await _repository.Add(product, cancellationToken);
    
    return created ? ProductResponse.FromModel(product) : null;
  }

  public Task<bool> Delete(string code, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
    {
      return Task.FromResult(false);
    }

    return _repository.Delete(code, cancellationToken);
  }

  public async Task<IReadOnlyList<ProductResponse>> GetAll(CancellationToken cancellationToken)
  {
    var products = await _repository.GetAll(cancellationToken);

    return products.Select(ProductResponse.FromModel).ToArray();
  }

  public async Task<ProductResponse?> GetByCode(string code, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
    {
      return null;
    }

    var product = await _repository.GetByCode(code, cancellationToken);
    return product is null ? null : ProductResponse.FromModel(product);
  }

  public async Task<ProductResponse?> Update(string code, UpdateProductRequest request, CancellationToken cancellationToken)
  {
    var product = new Product(code, request.Name, request.Price, request.Stock, request.Category);
    var updated = await _repository.Update(product, cancellationToken);

    return updated ? ProductResponse.FromModel(product) : null;
  }
}