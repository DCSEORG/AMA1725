![Header image](https://github.com/DougChisholm/App-Mod-Assist/blob/main/repo-header.png)

# Expense Management System - Modernized

A modern, cloud-native expense management application built on Azure, automatically generated from legacy application screenshots and database schema using GitHub Copilot.

## 🎯 What This Demonstrates

This repository showcases how GitHub Copilot can transform legacy applications into modern cloud-native solutions by:
- Analyzing legacy application screenshots
- Understanding database schemas
- Generating Azure infrastructure as code (Bicep)
- Creating modern ASP.NET Core applications
- Implementing REST APIs with Swagger
- Adding optional GenAI chat interfaces with Azure OpenAI

## 🚀 Quick Start

### For End Users (Deploy the Modernized App)

```bash
# 1. Clone this repository
git clone <repository-url>
cd AMA1725

# 2. Login to Azure
az login

# 3. Deploy everything with one command
./deploy.sh
```

**Access your app at**: `https://<your-app>.azurewebsites.net/Index`

📖 **Full deployment guide**: See [DEPLOYMENT.md](./DEPLOYMENT.md)

---

### For Contributors (Testing the Agent)

⚠️ **WARNING**: COLLABORATORS MUST FORK THE REPO EVERY TIME THEY RUN THE CODING AGENT TO AVOID POLLUTING THIS BASE TEMPLATE

**To test the modernization agent:**

1. **Fork this repository** (important: give it a unique name like `AMA-YourName-Test01`)
2. **Replace the inputs** (if testing with different legacy apps):
   - Replace files in `Legacy-Screenshots/` with your legacy app screenshots
   - Replace `Database-Schema/database_schema.sql` with your database schema
3. **Run the agent**: Open GitHub Copilot and tell it "modernise my app"
4. **Deploy locally**:
   ```bash
   git clone <your-fork>
   cd <your-fork>
   az login
   ./deploy.sh
   ```

---

## 📋 Features

### Implemented Functionality

✅ **Modern Web UI**
- Expense submission form with amount, date, category, description
- Expense listing with filtering
- Manager approval workflow
- Clean, responsive design with modern CSS

✅ **REST API**
- Full CRUD operations for expenses
- Swagger/OpenAPI documentation at `/swagger`
- Category and status endpoints
- Approval/rejection workflows

✅ **Infrastructure as Code**
- Bicep templates for all Azure resources
- App Service (B1 tier) in UK South
- Azure SQL Database (optional, Basic tier)
- Azure OpenAI + AI Search (optional, for GenAI chat)
- One-command deployment script

✅ **Optional GenAI Chat Interface**
- Natural language interaction with expense data
- RAG (Retrieval-Augmented Generation) pattern
- Powered by Azure OpenAI GPT-3.5-Turbo
- Context-aware responses using expense data

✅ **Deployment Options**
- Default: Dummy data (no database needed) - £10/month
- Optional: Database-backed with Azure SQL - £14/month
- Optional: Full GenAI features - £85/month

---

## 📁 What's Included

### Application Code (`src/ExpenseApp/`)
- ASP.NET Core 8.0 Razor Pages application
- Controllers for REST API
- Models matching the database schema
- Services for business logic (dummy data + optional GenAI)
- Modern, responsive UI

### Infrastructure (`infra/`)
- `main.bicep` - Orchestrates all resources
- `appservice.bicep` - App Service definition
- `azuresql.bicep` - Azure SQL configuration
- `azureopenai.bicep` - GenAI resources (optional)
- `create-managed-identity-user.sql` - Database security setup

### Deployment
- `deploy.sh` - Master deployment script
- `app.zip` - Pre-built application package (7.7MB)
- `.gitignore` - Configured to include deployment artifacts

### Documentation
- `DEPLOYMENT.md` - Complete deployment guide
- `ARCHITECTURE.md` - Architecture diagrams and details
- `chatui/README.md` - GenAI chat features and RAG pattern

---

## 🏗️ Architecture

The system uses Azure best practices with low-cost development SKUs:

```
User Browser → App Service → Dummy Data Service (default)
                    ↓
              Azure SQL DB (optional)
                    ↓
             Azure OpenAI (optional)
                    ↓
           Azure AI Search (optional)
```

See [ARCHITECTURE.md](./ARCHITECTURE.md) for detailed diagrams.

---

## 💡 How It Works

### Input (Legacy Application)
- 3 screenshots from a legacy expense system
- SQL Server database schema with tables for:
  - Users, Roles, Expenses, Categories, Status
- Requirements defined in `prompts/` folder

### Output (Modern Application)
- Cloud-native ASP.NET Core 8.0 web application
- REST API with Swagger documentation
- Azure Bicep infrastructure templates
- Optional GenAI chat interface
- Deployment automation
- Comprehensive documentation

### Agent Process
The GitHub Copilot agent follows these steps:
1. Reads all prompt files in sequence (see `prompts/prompt-order`)
2. Analyzes legacy screenshots and database schema
3. Creates Azure infrastructure (Bicep)
4. Generates modern ASP.NET Core application
5. Implements REST APIs with Swagger
6. Adds optional GenAI features
7. Creates deployment scripts and documentation
8. Builds and packages everything for deployment

---

## 📖 Documentation

- **[DEPLOYMENT.md](./DEPLOYMENT.md)** - Step-by-step deployment guide
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - System architecture and diagrams
- **[chatui/README.md](./chatui/README.md)** - GenAI chat and RAG pattern details
- **[prompts/](./prompts/)** - Agent instructions and workflow

---

## 🧪 Testing Scenarios

### Test Basic Functionality
1. Deploy with defaults (`./deploy.sh`)
2. Access `/Index` - view expenses
3. Click "Add Expense" - submit new expense
4. Click "Approve Expenses" - approve pending items
5. Access `/swagger` - test API endpoints

### Test GenAI Chat (Optional)
1. Edit `deploy.sh`: set `INCLUDE_CHAT_UI="true"`
2. Redeploy
3. Access `/Chat`
4. Try queries: "Show pending expenses", "Total approved amount"

---

## 💰 Cost Breakdown

| Configuration | Monthly Cost (approx) |
|--------------|----------------------|
| **Basic** (App Service + Dummy Data) | £10 |
| **+ Database** (Add Azure SQL) | £14 |
| **+ GenAI** (Add OpenAI + Search) | £85 |

All resources use lowest-cost development SKUs in UK South.

---

## 🔐 Security & Best Practices

✅ Managed Identity for Azure SQL authentication  
✅ HTTPS-only traffic  
✅ Minimal privilege access  
✅ UK South region (data residency)  
✅ No hardcoded secrets  
✅ Azure AD integration ready  

---

## 🛠️ Customization

Want to modify the generated application?

1. **Change UI**: Edit `src/ExpenseApp/Pages/*.cshtml`
2. **Change API**: Edit `src/ExpenseApp/Controllers/*.cs`
3. **Change Infrastructure**: Edit `infra/*.bicep`
4. **Rebuild**: 
   ```bash
   cd src/ExpenseApp
   dotnet publish -c Release -o ../../publish
   cd ../../publish && zip -r ../app.zip .
   ```
5. **Redeploy**: `./deploy.sh`

---

## 📚 Learn More

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Bicep IaC](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)

---

## 📝 License

See [LICENSE](./LICENSE) file for details.

---

**Built with ❤️ using GitHub Copilot and Azure best practices**
