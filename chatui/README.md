# GenAI Chat UI for Expense Management System

This folder contains the GenAI-powered chat interface that allows users to interact with the Expense Management System using natural language.

## Features

- **Natural Language Interface**: Ask questions about expenses in plain English
- **Context-Aware Responses**: Uses Retrieval-Augmented Generation (RAG) pattern to provide accurate, context-specific answers
- **Azure OpenAI Integration**: Powered by GPT-3.5-Turbo deployed in UK South
- **Expense Operations**: Perform actions like viewing, creating, and approving expenses through conversation

## Architecture

### RAG Pattern Implementation

The chat UI implements the Retrieval-Augmented Generation pattern:

1. **Retrieval Phase**: 
   - Fetches current expense data from the system
   - Retrieves available categories and statuses
   - Gathers user context and permissions

2. **Augmentation Phase**:
   - Enriches the AI prompt with retrieved expense data
   - Adds system context about available operations
   - Includes expense statistics and summaries

3. **Generation Phase**:
   - Sends enriched prompt to Azure OpenAI
   - GPT-3.5-Turbo generates contextually relevant responses
   - Returns natural language answers based on actual system data

### Components

- **Chat.cshtml.cs**: Page model handling chat logic and Azure OpenAI integration
- **Chat.cshtml**: Razor page for the chat UI
- **Azure OpenAI Client**: Handles API communication with Azure OpenAI service
- **Expense Service Integration**: Connects to expense data for context

## Configuration

The chat UI is controlled by the `IncludeChatUI` setting:

- **Default**: `false` (GenAI features disabled)
- **To Enable**: Set `INCLUDE_CHAT_UI="true"` in deploy.sh before deployment

### Required Azure Resources (when enabled)

- Azure OpenAI Account
- GPT-3.5-Turbo deployment (UK South)
- Azure AI Search (for enhanced RAG capabilities)

## Usage Examples

### Query Expenses
- "Show me all pending expenses"
- "What expenses were submitted this month?"
- "How many approved travel expenses do we have?"

### Expense Statistics
- "What's the total amount of submitted expenses?"
- "Show me expense breakdown by category"

### Create Expenses (Future Enhancement)
- "Create a travel expense for £120 dated today"
- "Add a meal expense for £25.50"

## Security Considerations

- API keys stored in Azure Key Vault (production)
- Managed Identity authentication for Azure services
- User context and permissions validated before operations
- No sensitive data exposed in prompts

## Future Enhancements

- Function calling to execute expense operations directly
- Multi-turn conversation with conversation history
- Advanced RAG with Azure AI Search for document retrieval
- Voice input/output capabilities
- Multi-language support

## Development

To test the chat UI locally:

1. Configure Azure OpenAI credentials in appsettings.json
2. Set `IncludeChatUI: true`
3. Run the application
4. Navigate to `/Chat`

## Deployment

The chat UI deploys automatically when `INCLUDE_CHAT_UI="true"` in deploy.sh. The deployment script:

1. Provisions Azure OpenAI resources
2. Deploys GPT-3.5-Turbo model
3. Configures app settings with endpoints and keys
4. Enables the chat route in the application

## Best Practices

- **Token Management**: Monitors and limits token usage
- **Error Handling**: Gracefully handles API failures
- **Rate Limiting**: Respects Azure OpenAI quotas
- **Prompt Engineering**: Uses clear, structured prompts for better results
- **Context Window**: Manages conversation context to stay within limits
