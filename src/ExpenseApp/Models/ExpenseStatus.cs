namespace ExpenseApp.Models;

public class ExpenseStatus
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
