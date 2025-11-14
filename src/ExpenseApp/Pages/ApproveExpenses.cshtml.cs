using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseApp.Services;
using ExpenseApp.Models;

namespace ExpenseApp.Pages;

public class ApproveExpensesModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public ApproveExpensesModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public List<Expense> PendingExpenses { get; set; } = new();
    public List<ExpenseCategory> Categories { get; set; } = new();
    public string FilterText { get; set; } = string.Empty;

    public async Task OnGetAsync(string? filter)
    {
        FilterText = filter ?? string.Empty;
        
        PendingExpenses = await _expenseService.GetPendingExpensesAsync();
        Categories = await _expenseService.GetCategoriesAsync();

        // Apply filter if provided
        if (!string.IsNullOrWhiteSpace(FilterText))
        {
            PendingExpenses = PendingExpenses.Where(e => 
                e.Description?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true ||
                GetCategoryName(e.CategoryId).Contains(FilterText, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }

    public async Task<IActionResult> OnPostApproveAsync(int expenseId)
    {
        await _expenseService.ApproveExpenseAsync(expenseId, 2); // Manager ID 2
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int expenseId)
    {
        await _expenseService.RejectExpenseAsync(expenseId, 2); // Manager ID 2
        return RedirectToPage();
    }

    public string GetCategoryName(int categoryId)
    {
        return Categories.FirstOrDefault(c => c.CategoryId == categoryId)?.CategoryName ?? "Unknown";
    }
}
