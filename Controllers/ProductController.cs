// using CrudApi.Data;
using CrudApi;
using CrudApi.DTOs;
using CrudApi.Models;
using CrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers;

[ApiController]
[Route("api/v1/products")]

public sealed class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken cancellationToken)
    {
      var products = await _service.GetAll(cancellationToken);
      return Ok(products);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetByCode([FromRoute] string code, CancellationToken cancellationToken)
    {
      var product = await _service.GetByCode(code, cancellationToken);

      return product is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found") : Ok(product);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
      try
        {
          var created = await _service.Create(request, cancellationToken);

          if (created is null)
          {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Product code already exists");
          }

          return CreatedAtAction(nameof(GetByCode), new { code = created.Code }, created);
      }catch (ArgumentException e){
          return Problem(
              statusCode: StatusCodes.Status400BadRequest,
              title: "Invalid product data!",
              detail: e.Message);
      }
    }

    [HttpDelete("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string code, CancellationToken cancellationToken)
    {
        var deleted = await _service.Delete(code, cancellationToken);

        return deleted ? NoContent() : Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Product not found",
            detail: $"No product was found for code {code}.");
    }

    [HttpPut("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update([FromRoute] string code, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var updated = await _service.Update(code, request, cancellationToken);



        return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: $"No product was found for code {code}.") : Ok(updated);
      }
      catch (ArgumentException e)
      {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid product data!",
            detail: e.Message);
      }
    }
}