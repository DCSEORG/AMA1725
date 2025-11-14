# Expense Management System - Deployment Guide

A modern, cloud-native expense management application built on Azure, generated from legacy application screenshots and database schema.

## 🚀 Quick Start

### Prerequisites
- Azure CLI installed and configured (`az --version`)
- Active Azure subscription
- Logged in to Azure (`az login`)

### Simple Deployment (No GenAI)

```bash
# Clone the repository
git clone <repository-url>
cd AMA1725

# Run deployment script
./deploy.sh
```

**That's it!** The script will:
1. Create an Azure resource group
2. Deploy App Service infrastructure
3. Deploy the application
4. Provide you with the URL

**Access the app**: Navigate to `https://<your-app>.azurewebsites.net/Index`

⚠️ **IMPORTANT**: Don't forget to add `/Index` to the URL (not just the root)

---

## 📋 Features

### Core Functionality
- ✅ **Expense Submission**: Add new expenses with amount, date, category, and description
- ✅ **Expense Viewing**: List and filter all expenses
- ✅ **Expense Approval**: Manager workflow to approve/reject pending expenses
- ✅ **REST API**: Full API with Swagger documentation at `/swagger`
- ✅ **Modern UI**: Clean, responsive interface

### Optional GenAI Features
- 🤖 **Natural Language Chat**: Interact with expense system using plain English
- 🔍 **RAG Pattern**: Context-aware responses using Retrieval-Augmented Generation
- 🧠 **Azure OpenAI**: Powered by GPT-3.5-Turbo

---

## 🎯 Deployment Options

### Option 1: Basic Deployment (Default, Recommended for Testing)

**What you get:**
- App Service with dummy data
- Full UI and API functionality
- No database costs
- ~£10/month

**Command:**
```bash
./deploy.sh
```

The default configuration uses in-memory dummy data, so you can test all features without provisioning a database.

---

### Option 2: Deployment with GenAI Chat UI

**What you get:**
- Everything from Option 1
- Azure OpenAI with GPT-3.5-Turbo
- Natural language chat interface
- Azure AI Search for RAG
- ~£85/month

**Steps:**

1. Edit `deploy.sh` and change:
   ```bash
   INCLUDE_CHAT_UI="true"  # Change from "false" to "true"
   ```

2. Run deployment:
   ```bash
   ./deploy.sh
   ```

3. Access chat at: `https://<your-app>.azurewebsites.net/Chat`

---

### Option 3: Database-Backed Deployment (Future)

Currently, the app uses dummy data by default. To use Azure SQL:

1. The infrastructure is already provisioned (Azure SQL is deployed)
2. Initialize the database:
   ```bash
   # Connect to your SQL Server
   az sql db show-connection-string --name ExpenseDB --server <server-name>
   
   # Run the schema script
   # Execute Database-Schema/database_schema.sql
   ```

3. Create managed identity user:
   ```sql
   -- Run infra/create-managed-identity-user.sql
   -- Replace <webapp-name> with your actual app name
   ```

4. Update app configuration:
   - Set `UseDummyData: false` in the App Service configuration
   - Restart the app

---

## 🏗️ Architecture

See [ARCHITECTURE.md](./ARCHITECTURE.md) for detailed architecture diagrams and component descriptions.

**High-level components:**
- **Azure App Service**: Hosts the ASP.NET Core 8.0 application
- **Azure SQL Database**: Optional persistent storage
- **Azure OpenAI**: Optional natural language processing
- **Azure AI Search**: Optional RAG pattern support

---

## 📁 Repository Structure

```
├── Database-Schema/          # SQL schema and seed data
│   └── database_schema.sql   # Complete database setup
├── Legacy-Screenshots/        # Original app screenshots used for modernization
│   ├── exp1.png              # Add Expense screen
│   ├── exp2.png              # View Expenses screen
│   └── exp3.png              # Approve Expenses screen
├── infra/                    # Bicep infrastructure as code
│   ├── main.bicep            # Main orchestration file
│   ├── appservice.bicep      # App Service definition
│   ├── azuresql.bicep        # Azure SQL configuration
│   ├── azureopenai.bicep     # OpenAI resources (optional)
│   └── create-managed-identity-user.sql
├── src/ExpenseApp/           # Application source code
│   ├── Controllers/          # API controllers
│   ├── Models/               # Data models
│   ├── Pages/                # Razor pages (UI)
│   ├── Services/             # Business logic
│   └── wwwroot/              # Static assets (CSS)
├── chatui/                   # GenAI chat documentation
│   └── README.md             # Chat UI details and RAG pattern
├── deploy.sh                 # Master deployment script
├── app.zip                   # Pre-built deployment package
├── ARCHITECTURE.md           # Architecture diagrams
└── README.md                 # This file
```

---

## 🔧 Configuration

### App Service Settings

Edit these in `deploy.sh` before deployment:

```bash
RESOURCE_GROUP="rg-expenseapp-dev"     # Resource group name
LOCATION="uksouth"                      # Azure region
INCLUDE_CHAT_UI="false"                 # Enable/disable GenAI
```

### Application Settings

Set in Azure Portal or via CLI:

- `UseDummyData`: `true` (default) or `false` (use database)
- `IncludeChatUI`: `true` or `false` (matches deployment choice)
- `AzureOpenAI:Endpoint`: Auto-configured when GenAI enabled
- `AzureOpenAI:DeploymentName`: `gpt-35-turbo`

---

## 🧪 Testing

### Test the Web UI

1. Navigate to: `https://<your-app>.azurewebsites.net/Index`
2. Click "Add Expense" to create a new expense
3. View expenses on the main page
4. Click "Approve Expenses" to see pending expenses

### Test the API

1. Navigate to: `https://<your-app>.azurewebsites.net/swagger`
2. Try the API endpoints:
   - `GET /api/expenses` - Get all expenses
   - `GET /api/expenses/pending` - Get pending expenses
   - `POST /api/expenses` - Create new expense
   - `POST /api/expenses/{id}/approve` - Approve expense

### Test GenAI Chat (if enabled)

1. Navigate to: `https://<your-app>.azurewebsites.net/Chat`
2. Try queries like:
   - "Show me all pending expenses"
   - "What's the total amount of approved expenses?"
   - "List expenses by category"

---

## 🛠️ Customization

### Modify the UI

Edit files in `src/ExpenseApp/`:
- `Pages/*.cshtml` - Razor view templates
- `wwwroot/css/site.css` - Styling

### Modify the API

Edit files in `src/ExpenseApp/Controllers/`:
- `ExpensesController.cs` - Expense endpoints
- `CategoriesController.cs` - Category endpoints

### Rebuild and Redeploy

```bash
cd src/ExpenseApp
dotnet publish --configuration Release --output ../../publish
cd ../../publish
zip -r ../app.zip .
cd ..
./deploy.sh
```

---

## 📊 Costs

### Basic Deployment (Default)
- App Service (B1): ~£10/month
- **Total: ~£10/month**

### With GenAI Chat
- App Service (B1): ~£10/month
- Azure OpenAI (S0): ~£15/month
- Azure AI Search (Basic): ~£60/month
- **Total: ~£85/month**

### With Database (Optional)
- Add Azure SQL (Basic): ~£4/month

All pricing is approximate and for UK South region. See [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/) for exact costs.

---

## 🔐 Security

- ✅ **HTTPS Only**: All traffic encrypted
- ✅ **Managed Identity**: No passwords in code
- ✅ **Azure AD**: Authentication for SQL (when enabled)
- ✅ **Minimal Permissions**: Least privilege access
- ✅ **UK Data Residency**: All resources in UK South

---

## 🐛 Troubleshooting

### App doesn't load
- Ensure you navigate to `/Index` not just the root URL
- Check App Service is running in Azure Portal
- Review Application Insights logs

### Chat UI not available
- Verify `INCLUDE_CHAT_UI="true"` was set before deployment
- Check App Service configuration has `IncludeChatUI: true`
- Redeploy if you changed the setting after initial deployment

### Database connection errors
- If using Azure SQL, verify managed identity user was created
- Check connection string in App Service configuration
- Ensure `UseDummyData` is set correctly

### Deployment fails
- Ensure Azure CLI is logged in: `az account show`
- Check you have permissions to create resources
- Verify subscription has available quota

---

## 📞 Support

For issues or questions:
1. Check [ARCHITECTURE.md](./ARCHITECTURE.md) for system design
2. Review [chatui/README.md](./chatui/README.md) for GenAI features
3. Check Azure Portal logs in Application Insights
4. Review Swagger API docs at `/swagger`

---

## 📝 License

See [LICENSE](./LICENSE) file for details.

---

## 🎓 Learning Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Bicep Documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)
- [RAG Pattern Guide](https://learn.microsoft.com/azure/search/retrieval-augmented-generation-overview)

---

**Built with ❤️ using GitHub Copilot and Azure best practices**
