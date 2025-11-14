namespace ExpenseApp.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Role Role { get; set; } = null!;
    public User? Manager { get; set; }
    public ICollection<User> DirectReports { get; set; } = new List<User>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Expense> ReviewedExpenses { get; set; } = new List<Expense>();
}
