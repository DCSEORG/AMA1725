using ExpenseApp.Models;

namespace ExpenseApp.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetAllExpensesAsync();
    Task<List<Expense>> GetExpensesByUserAsync(int userId);
    Task<List<Expense>> GetPendingExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int expenseId);
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task<Expense> UpdateExpenseAsync(Expense expense);
    Task<bool> ApproveExpenseAsync(int expenseId, int reviewerId);
    Task<bool> RejectExpenseAsync(int expenseId, int reviewerId);
    Task<List<ExpenseCategory>> GetCategoriesAsync();
    Task<List<ExpenseStatus>> GetStatusesAsync();
}

public class DummyExpenseService : IExpenseService
{
    private static List<Expense> _expenses = new();
    private static List<ExpenseCategory> _categories = new();
    private static List<ExpenseStatus> _statuses = new();
    private static int _nextId = 1;

    static DummyExpenseService()
    {
        // Initialize dummy data
        _categories = new List<ExpenseCategory>
        {
            new() { CategoryId = 1, CategoryName = "Travel", IsActive = true },
            new() { CategoryId = 2, CategoryName = "Meals", IsActive = true },
            new() { CategoryId = 3, CategoryName = "Supplies", IsActive = true },
            new() { CategoryId = 4, CategoryName = "Accommodation", IsActive = true },
            new() { CategoryId = 5, CategoryName = "Other", IsActive = true }
        };

        _statuses = new List<ExpenseStatus>
        {
            new() { StatusId = 1, StatusName = "Draft" },
            new() { StatusId = 2, StatusName = "Submitted" },
            new() { StatusId = 3, StatusName = "Approved" },
            new() { StatusId = 4, StatusName = "Rejected" }
        };

        // Sample expenses
        _expenses = new List<Expense>
        {
            new()
            {
                ExpenseId = _nextId++,
                UserId = 1,
                CategoryId = 1,
                StatusId = 2,
                AmountMinor = 12000,
                Currency = "GBP",
                ExpenseDate = new DateTime(2024, 1, 15),
                Description = "Client meeting travel",
                SubmittedAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new()
            {
                ExpenseId = _nextId++,
                UserId = 1,
                CategoryId = 2,
                StatusId = 2,
                AmountMinor = 6900,
                Currency = "GBP",
                ExpenseDate = new DateTime(2023, 1, 10),
                Description = "Team lunch",
                SubmittedAt = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new()
            {
                ExpenseId = _nextId++,
                UserId = 1,
                CategoryId = 3,
                StatusId = 3,
                AmountMinor = 9950,
                Currency = "GBP",
                ExpenseDate = new DateTime(2023, 12, 4),
                Description = "Office supplies for project",
                SubmittedAt = DateTime.UtcNow.AddDays(-10),
                ReviewedAt = DateTime.UtcNow.AddDays(-8),
                ReviewedBy = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            },
            new()
            {
                ExpenseId = _nextId++,
                UserId = 1,
                CategoryId = 1,
                StatusId = 3,
                AmountMinor = 1920,
                Currency = "GBP",
                ExpenseDate = new DateTime(2023, 21, 18),
                Description = "Transport to conference",
                SubmittedAt = DateTime.UtcNow.AddDays(-15),
                ReviewedAt = DateTime.UtcNow.AddDays(-14),
                ReviewedBy = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            }
        };
    }

    public Task<List<Expense>> GetAllExpensesAsync()
    {
        return Task.FromResult(_expenses);
    }

    public Task<List<Expense>> GetExpensesByUserAsync(int userId)
    {
        return Task.FromResult(_expenses.Where(e => e.UserId == userId).ToList());
    }

    public Task<List<Expense>> GetPendingExpensesAsync()
    {
        return Task.FromResult(_expenses.Where(e => e.StatusId == 2).ToList());
    }

    public Task<Expense?> GetExpenseByIdAsync(int expenseId)
    {
        return Task.FromResult(_expenses.FirstOrDefault(e => e.ExpenseId == expenseId));
    }

    public Task<Expense> CreateExpenseAsync(Expense expense)
    {
        expense.ExpenseId = _nextId++;
        expense.CreatedAt = DateTime.UtcNow;
        expense.StatusId = 1; // Draft
        _expenses.Add(expense);
        return Task.FromResult(expense);
    }

    public Task<Expense> UpdateExpenseAsync(Expense expense)
    {
        var existing = _expenses.FirstOrDefault(e => e.ExpenseId == expense.ExpenseId);
        if (existing != null)
        {
            existing.CategoryId = expense.CategoryId;
            existing.AmountMinor = expense.AmountMinor;
            existing.ExpenseDate = expense.ExpenseDate;
            existing.Description = expense.Description;
            
            // Submit if changing to submitted status
            if (expense.StatusId == 2 && existing.StatusId != 2)
            {
                existing.SubmittedAt = DateTime.UtcNow;
            }
            existing.StatusId = expense.StatusId;
        }
        return Task.FromResult(existing ?? expense);
    }

    public Task<bool> ApproveExpenseAsync(int expenseId, int reviewerId)
    {
        var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
        if (expense != null && expense.StatusId == 2)
        {
            expense.StatusId = 3; // Approved
            expense.ReviewedBy = reviewerId;
            expense.ReviewedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> RejectExpenseAsync(int expenseId, int reviewerId)
    {
        var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
        if (expense != null && expense.StatusId == 2)
        {
            expense.StatusId = 4; // Rejected
            expense.ReviewedBy = reviewerId;
            expense.ReviewedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<List<ExpenseCategory>> GetCategoriesAsync()
    {
        return Task.FromResult(_categories);
    }

    public Task<List<ExpenseStatus>> GetStatusesAsync()
    {
        return Task.FromResult(_statuses);
    }
}
