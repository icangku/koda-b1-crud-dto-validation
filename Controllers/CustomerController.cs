// using CrudApi.Data;
using CrudApi;
using CrudApi.DTOs;
using CrudApi.Models;
using CrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers;

[ApiController]
[Route("api/v1/customers")]

public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Customer>>> GetAll(CancellationToken cancellationToken)
    {
      var customers = await _service.GetAll(cancellationToken);
      return Ok(customers);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetByCode([FromRoute] string code, CancellationToken cancellationToken)
    {
      var customer = await _service.GetByCode(code, cancellationToken);

      return customer is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer not found") : Ok(customer);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
      try
        {
          var created = await _service.Create(request, cancellationToken);

          if (created is null)
          {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Customer id already exists");
          }

          return CreatedAtAction(nameof(GetByCode), new { code = created.Id }, created);
      }catch (ArgumentException e){
          return Problem(
              statusCode: StatusCodes.Status400BadRequest,
              title: "Invalid customer data!",
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
            title: "Customer not found",
            detail: $"No customer was found for code {code}.");
    }

    [HttpPut("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> Update([FromRoute] string code, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var updated = await _service.Update(code, request, cancellationToken);



        return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer not found", detail: $"No customer was found for code {code}.") : Ok(updated);
      }
      catch (ArgumentException e)
      {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid customer data!",
            detail: e.Message);
      }
    }
}