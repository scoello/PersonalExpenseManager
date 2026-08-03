using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Application;
namespace PersonalExpenses.Api.Controllers;

[ApiController, Authorize, Route("api/expenses")]
public sealed class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    /// <summary>
    /// Get a List of Expenses
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>The user's expenses</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExpenseDto>>> List(CancellationToken ct)
    {
        var list = await expenseService.ListAsync(UserId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Get an specific Expense
    /// </summary>
    /// <param name="id">The id of the expense</param>
    /// <param name="ct"></param>
    /// <returns>The expense recovered</returns>
    [HttpGet("{id:guid}")] 
    public async Task<ActionResult<ExpenseDto>> Get(Guid id, CancellationToken ct)
    {
        var expense = await expenseService.GetAsync(id, UserId, ct);
        return expense is { } x ? Ok(x) : NotFound();
    }    

    /// <summary>
    /// Creates an expense
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="ct"></param>
    /// <returns>201</returns>
    [HttpPost] 
    public async Task<ActionResult<ExpenseDto>> Create(SaveExpenseRequest request, CancellationToken ct) 
    { 
        var x = await expenseService.CreateAsync(UserId, request, ct); 
        //Http 201
        return CreatedAtAction(nameof(Get), new { id = x.Id }, x); 
    }

    /// <summary>
    /// Updates an expense
    /// </summary>
    /// <param name="id">Id of the expense</param>
    /// <param name="request">Expense's request</param>
    /// <param name="ct"></param>
    /// <returns>204 or 404</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, SaveExpenseRequest request, CancellationToken ct)
    {
        var result = await expenseService.UpdateAsync(id, UserId, request, ct);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes an expense
    /// </summary>
    /// <param name="id">Id of the expense</param>
    /// <param name="ct"></param>
    /// <returns>204 or 404</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await expenseService.DeleteAsync(id, UserId, ct);
        return result ? NoContent() : NotFound();
    }
}
