using Microsoft.EntityFrameworkCore;
using ExpenseApp.Models;

namespace ExpenseApp.Data;

public class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<ExpenseStatus> ExpenseStatuses => Set<ExpenseStatus>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User self-referencing relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.DirectReports)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Expense reviewer relationship
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Reviewer)
            .WithMany(u => u.ReviewedExpenses)
            .HasForeignKey(e => e.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Table names to match SQL schema
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<ExpenseCategory>().ToTable("ExpenseCategories");
        modelBuilder.Entity<ExpenseStatus>().ToTable("ExpenseStatus");
        modelBuilder.Entity<Expense>().ToTable("Expenses");
    }
}
