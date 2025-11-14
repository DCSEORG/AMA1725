using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseApp.Services;
using ExpenseApp.Models;

namespace ExpenseApp.Pages;

public class IndexModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public IndexModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public List<Expense> Expenses { get; set; } = new();
    public List<ExpenseCategory> Categories { get; set; } = new();
    public List<ExpenseStatus> Statuses { get; set; } = new();
    public string FilterText { get; set; } = string.Empty;

    public async Task OnGetAsync(string? filter)
    {
        FilterText = filter ?? string.Empty;
        
        Expenses = await _expenseService.GetAllExpensesAsync();
        Categories = await _expenseService.GetCategoriesAsync();
        Statuses = await _expenseService.GetStatusesAsync();

        // Apply filter if provided
        if (!string.IsNullOrWhiteSpace(FilterText))
        {
            Expenses = Expenses.Where(e => 
                e.Description?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true ||
                GetCategoryName(e.CategoryId).Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                GetStatusName(e.StatusId).Contains(FilterText, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }

    public string GetCategoryName(int categoryId)
    {
        return Categories.FirstOrDefault(c => c.CategoryId == categoryId)?.CategoryName ?? "Unknown";
    }

    public string GetStatusName(int statusId)
    {
        return Statuses.FirstOrDefault(s => s.StatusId == statusId)?.StatusName ?? "Unknown";
    }

    public string GetStatusClass(int statusId)
    {
        return statusId switch
        {
            1 => "status-draft",
            2 => "status-submitted",
            3 => "status-approved",
            4 => "status-rejected",
            _ => ""
        };
    }
}
