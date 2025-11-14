namespace ExpenseApp.Models;

public class ExpenseCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
