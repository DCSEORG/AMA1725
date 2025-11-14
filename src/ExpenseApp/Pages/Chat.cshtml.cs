using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.AI.OpenAI;
using Azure;
using ExpenseApp.Services;

namespace ExpenseApp.Pages;

public class ChatModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ChatModel> _logger;

    public ChatModel(IConfiguration configuration, IExpenseService expenseService, ILogger<ChatModel> logger)
    {
        _configuration = configuration;
        _expenseService = expenseService;
        _logger = logger;
    }

    [BindProperty]
    public string UserMessage { get; set; } = string.Empty;

    public List<ChatMessage> ChatHistory { get; set; } = new();

    public bool IsChatUIEnabled { get; set; }

    public async Task OnGetAsync()
    {
        IsChatUIEnabled = _configuration.GetValue<bool>("IncludeChatUI", false);
        
        if (!IsChatUIEnabled)
        {
            ViewData["Message"] = "GenAI Chat UI is not enabled. Set IncludeChatUI=true in configuration.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        IsChatUIEnabled = _configuration.GetValue<bool>("IncludeChatUI", false);
        
        if (!IsChatUIEnabled)
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(UserMessage))
        {
            return Page();
        }

        try
        {
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                ViewData["Error"] = "Azure OpenAI is not configured properly.";
                return Page();
            }

            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            // Build context from expense data
            var expenses = await _expenseService.GetAllExpensesAsync();
            var categories = await _expenseService.GetCategoriesAsync();
            
            var systemPrompt = @"You are an AI assistant for an Expense Management System. 
You can help users with expense-related queries and perform operations like:
- Viewing expenses
- Creating new expenses
- Approving/rejecting expenses
- Querying expense statistics

Use natural language to help users interact with the expense system.
Available expense categories: " + string.Join(", ", categories.Select(c => c.CategoryName)) + @"

Current expense summary: " + expenses.Count + " total expenses.";

            var chatMessages = new List<Azure.AI.OpenAI.ChatRequestMessage>
            {
                new ChatRequestSystemMessage(systemPrompt),
                new ChatRequestUserMessage(UserMessage)
            };

            var chatOptions = new ChatCompletionsOptions(deploymentName, chatMessages)
            {
                Temperature = 0.7f,
                MaxTokens = 800
            };

            var response = await client.GetChatCompletionsAsync(chatOptions);
            var assistantResponse = response.Value.Choices[0].Message.Content;

            ChatHistory.Add(new ChatMessage { Role = "user", Content = UserMessage });
            ChatHistory.Add(new ChatMessage { Role = "assistant", Content = assistantResponse });

            ViewData["Response"] = assistantResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI");
            ViewData["Error"] = $"Error: {ex.Message}";
        }

        return Page();
    }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
