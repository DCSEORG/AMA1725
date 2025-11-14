using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseApp.Services;
using ExpenseApp.Models;

namespace ExpenseApp.Pages;

public class AddExpenseModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public AddExpenseModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [BindProperty]
    public decimal Amount { get; set; }

    [BindProperty]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [BindProperty]
    public int CategoryId { get; set; } = 1;

    [BindProperty]
    public string? Description { get; set; }

    public List<ExpenseCategory> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _expenseService.GetCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Categories = await _expenseService.GetCategoriesAsync();
            return Page();
        }

        var expense = new Expense
        {
            UserId = 1, // Default user for demo
            CategoryId = CategoryId,
            AmountMinor = (int)(Amount * 100), // Convert pounds to pence
            Currency = "GBP",
            ExpenseDate = ExpenseDate,
            Description = Description
        };

        await _expenseService.CreateExpenseAsync(expense);
        
        return RedirectToPage("/Index");
    }
}
