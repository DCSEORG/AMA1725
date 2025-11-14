namespace ExpenseApp.Models;

public class Expense
{
    public int ExpenseId { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }
    public int AmountMinor { get; set; } // Amount in pence
    public string Currency { get; set; } = "GBP";
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFile { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; } = null!;
    public ExpenseCategory Category { get; set; } = null!;
    public ExpenseStatus Status { get; set; } = null!;
    public User? Reviewer { get; set; }
    
    // Helper property to get amount in pounds
    public decimal Amount => AmountMinor / 100m;
}
