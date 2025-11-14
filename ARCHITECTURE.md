# Azure Services Architecture Diagram

## Expense Management System - Cloud Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                           User / Browser                             │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             │ HTTPS
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      Azure App Service                               │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  ASP.NET Core 8.0 Web Application                          │    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐    │    │
│  │  │ Razor Pages  │  │  REST APIs   │  │  Chat UI     │    │    │
│  │  │   (UI)       │  │  (Swagger)   │  │  (Optional)  │    │    │
│  │  └──────────────┘  └──────────────┘  └──────────────┘    │    │
│  │                                                            │    │
│  │  SKU: B1 (Basic)                                          │    │
│  │  Region: UK South                                         │    │
│  │  Managed Identity: Enabled                                │    │
│  └────────────────────────────────────────────────────────────┘    │
└──────┬────────────────────────────┬────────────────────────┬────────┘
       │                            │                        │
       │ Managed Identity           │ REST API               │ API Key
       │ Authentication             │                        │ (when enabled)
       ▼                            ▼                        ▼
┌──────────────────┐      ┌──────────────────┐    ┌────────────────────┐
│  Azure SQL DB    │      │ Dummy Data       │    │ Azure OpenAI       │
│                  │      │ Service          │    │  (Optional)        │
│ Database:        │      │ (Default)        │    │                    │
│  ExpenseDB       │      │                  │    │ Model: GPT-3.5     │
│                  │      │ In-Memory        │    │ Deployment:        │
│ SKU: Basic (5GB) │      │ Mock Data        │    │  gpt-35-turbo      │
│ Region: UK South │      │                  │    │                    │
│                  │      └──────────────────┘    │ SKU: S0            │
│ Managed Identity │                              │ Region: UK South   │
│  User Created    │                              │                    │
└──────────────────┘                              └─────────┬──────────┘
                                                            │
                                                            │ Search Index
                                                            │ (RAG Pattern)
                                                            ▼
                                                  ┌────────────────────┐
                                                  │ Azure AI Search    │
                                                  │  (Optional)        │
                                                  │                    │
                                                  │ SKU: Basic         │
                                                  │ Region: UK South   │
                                                  │                    │
                                                  │ Context Retrieval  │
                                                  └────────────────────┘
```

## Component Details

### Azure App Service (Always Deployed)
- **Purpose**: Hosts the ASP.NET Core web application
- **Features**: 
  - Razor Pages UI for expense management
  - REST API with Swagger documentation
  - Optional GenAI chat interface
- **SKU**: B1 (Basic) - Low-cost development tier
- **Region**: UK South
- **Authentication**: Managed Identity enabled for Azure SQL

### Azure SQL Database (Configured, Optional Use)
- **Purpose**: Persistent storage for expense data
- **Default Mode**: Application uses dummy data (UseDummyData=true)
- **Database**: ExpenseDB
- **Schema**: Users, Roles, Expenses, Categories, Status tables
- **SKU**: Basic tier (5GB storage)
- **Authentication**: Managed Identity (App Service connects as database user)

### Dummy Data Service (Default)
- **Purpose**: Provides mock expense data without database dependency
- **Mode**: In-memory data storage
- **Benefits**: 
  - No database connection required
  - Instant deployment and testing
  - Zero database costs for demos

### Azure OpenAI (Optional - Default: Disabled)
- **Enabled When**: IncludeChatUI = true
- **Purpose**: Natural language processing for chat interface
- **Model**: GPT-3.5-Turbo (gpt-35-turbo)
- **SKU**: Standard (S0)
- **Region**: UK South
- **Capacity**: 10 TPM (Tokens Per Minute) for development

### Azure AI Search (Optional - Default: Disabled)
- **Enabled When**: IncludeChatUI = true
- **Purpose**: RAG (Retrieval-Augmented Generation) pattern
- **Features**:
  - Context retrieval for GenAI responses
  - Expense data indexing
  - Semantic search capabilities
- **SKU**: Basic tier
- **Region**: UK South

## Data Flow

### Standard Expense Operations (Without GenAI)
1. User accesses App Service via browser (HTTPS)
2. Razor Pages render expense UI
3. User submits/views expenses
4. App Service queries Dummy Data Service
5. Results returned and displayed

### GenAI Chat Operations (When Enabled)
1. User sends natural language query via Chat UI
2. App Service retrieves relevant context:
   - Current expenses from Dummy Data Service
   - Optional: Additional context from Azure AI Search
3. Context + query sent to Azure OpenAI
4. GPT-3.5-Turbo generates response
5. Response displayed in chat interface

### API Operations
1. External client calls REST API endpoints
2. App Service processes request
3. Data retrieved from Dummy Data Service
4. JSON response returned with Swagger documentation

## Security

- **HTTPS Only**: All traffic encrypted
- **Managed Identity**: No connection strings or passwords
- **Azure AD Integration**: For database authentication (when enabled)
- **API Keys**: Stored in App Service configuration (for OpenAI)
- **Network**: Public endpoints with Azure security controls

## Deployment Options

### Option 1: Basic (Default)
- App Service + Dummy Data
- No GenAI features
- Lowest cost
- Instant deployment

### Option 2: Full GenAI (Set IncludeChatUI=true)
- App Service + Dummy Data + Azure OpenAI + Azure AI Search
- Complete natural language interface
- Higher cost (OpenAI + Search)
- Advanced features

### Option 3: Database-Backed (Future)
- App Service + Azure SQL + Optional GenAI
- Persistent data storage
- Production-ready
- Moderate cost

## Cost Optimization

All resources use lowest-cost development SKUs:
- App Service: B1 (~£10/month)
- Azure SQL: Basic (~£4/month) - only when used
- Azure OpenAI: S0 (~£15/month) - only when enabled
- Azure AI Search: Basic (~£60/month) - only when enabled

**Default deployment cost**: ~£10/month (App Service only with dummy data)
