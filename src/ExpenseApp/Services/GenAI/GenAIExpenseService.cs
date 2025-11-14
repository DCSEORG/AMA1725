using Azure.AI.OpenAI;
using Azure;
using ExpenseApp.Models;
using System.Text.Json;

namespace ExpenseApp.Services.GenAI;

/// <summary>
/// Service that combines Expense APIs with GenAI functionality
/// Enables natural language interaction with expense operations
/// </summary>
public interface IGenAIExpenseService
{
    Task<string> ProcessNaturalLanguageQueryAsync(string userQuery);
}

public class GenAIExpenseService : IGenAIExpenseService
{
    private readonly IExpenseService _expenseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GenAIExpenseService> _logger;

    public GenAIExpenseService(
        IExpenseService expenseService,
        IConfiguration configuration,
        ILogger<GenAIExpenseService> logger)
    {
        _expenseService = expenseService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> ProcessNaturalLanguageQueryAsync(string userQuery)
    {
        try
        {
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                return "GenAI is not configured. Please set Azure OpenAI credentials.";
            }

            // Fetch context data
            var expenses = await _expenseService.GetAllExpensesAsync();
            var categories = await _expenseService.GetCategoriesAsync();
            var pendingExpenses = await _expenseService.GetPendingExpensesAsync();

            // Build context
            var context = BuildExpenseContext(expenses, categories, pendingExpenses);

            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            var systemMessage = @"You are an AI assistant for an Expense Management System.
You can help users query and analyze expense data. 

Available functions:
- Get all expenses
- Get pending expenses  
- Get expense statistics
- Query expenses by category or status

Provide helpful, accurate answers based on the expense data provided in the context.
Format monetary amounts as £X.XX (GBP).";

            var messages = new List<ChatRequestMessage>
            {
                new ChatRequestSystemMessage(systemMessage + "\n\nCurrent Expense Context:\n" + context),
                new ChatRequestUserMessage(userQuery)
            };

            var chatOptions = new ChatCompletionsOptions(deploymentName, messages)
            {
                Temperature = 0.7f,
                MaxTokens = 1000
            };

            var response = await client.GetChatCompletionsAsync(chatOptions);
            return response.Value.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing natural language query");
            return $"Error: {ex.Message}";
        }
    }

    private string BuildExpenseContext(
        List<Expense> expenses,
        List<ExpenseCategory> categories,
        List<Expense> pendingExpenses)
    {
        var totalAmount = expenses.Sum(e => e.Amount);
        var approvedAmount = expenses.Where(e => e.StatusId == 3).Sum(e => e.Amount);
        var pendingAmount = pendingExpenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.CategoryId)
            .Select(g => new
            {
                Category = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.CategoryName ?? "Unknown",
                Count = g.Count(),
                Total = g.Sum(e => e.Amount)
            });

        var context = $@"
Total Expenses: {expenses.Count}
Total Amount: £{totalAmount:F2}
Approved Amount: £{approvedAmount:F2}
Pending Expenses: {pendingExpenses.Count} (£{pendingAmount:F2})

Breakdown by Category:
{string.Join("\n", byCategory.Select(c => $"- {c.Category}: {c.Count} expenses, £{c.Total:F2}"))}

Available Categories: {string.Join(", ", categories.Select(c => c.CategoryName))}
";
        return context;
    }
}
