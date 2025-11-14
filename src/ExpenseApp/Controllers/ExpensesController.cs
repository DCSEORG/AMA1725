using Microsoft.AspNetCore.Mvc;
using ExpenseApp.Models;
using ExpenseApp.Services;

namespace ExpenseApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    /// <summary>
    /// Get all expenses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Expense>>> GetAllExpenses()
    {
        var expenses = await _expenseService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    /// <summary>
    /// Get expenses by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Expense>>> GetExpensesByUser(int userId)
    {
        var expenses = await _expenseService.GetExpensesByUserAsync(userId);
        return Ok(expenses);
    }

    /// <summary>
    /// Get pending expenses for approval
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<List<Expense>>> GetPendingExpenses()
    {
        var expenses = await _expenseService.GetPendingExpensesAsync();
        return Ok(expenses);
    }

    /// <summary>
    /// Get expense by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        return Ok(expense);
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        var expense = new Expense
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            AmountMinor = request.AmountMinor,
            Currency = request.Currency ?? "GBP",
            ExpenseDate = request.ExpenseDate,
            Description = request.Description
        };

        var created = await _expenseService.CreateExpenseAsync(expense);
        return CreatedAtAction(nameof(GetExpense), new { id = created.ExpenseId }, created);
    }

    /// <summary>
    /// Submit an expense for approval
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<Expense>> SubmitExpense(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.StatusId = 2; // Submitted
        expense.SubmittedAt = DateTime.UtcNow;
        var updated = await _expenseService.UpdateExpenseAsync(expense);
        return Ok(updated);
    }

    /// <summary>
    /// Approve an expense
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult> ApproveExpense(int id, [FromBody] ReviewRequest request)
    {
        var success = await _expenseService.ApproveExpenseAsync(id, request.ReviewerId);
        if (!success)
        {
            return NotFound();
        }
        return Ok(new { message = "Expense approved successfully" });
    }

    /// <summary>
    /// Reject an expense
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult> RejectExpense(int id, [FromBody] ReviewRequest request)
    {
        var success = await _expenseService.RejectExpenseAsync(id, request.ReviewerId);
        if (!success)
        {
            return NotFound();
        }
        return Ok(new { message = "Expense rejected successfully" });
    }

    /// <summary>
    /// Update an expense
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Expense>> UpdateExpense(int id, [FromBody] UpdateExpenseRequest request)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.CategoryId = request.CategoryId;
        expense.AmountMinor = request.AmountMinor;
        expense.ExpenseDate = request.ExpenseDate;
        expense.Description = request.Description;

        var updated = await _expenseService.UpdateExpenseAsync(expense);
        return Ok(updated);
    }
}

public class CreateExpenseRequest
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int AmountMinor { get; set; }
    public string? Currency { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
}

public class UpdateExpenseRequest
{
    public int CategoryId { get; set; }
    public int AmountMinor { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
}

public class ReviewRequest
{
    public int ReviewerId { get; set; }
}
