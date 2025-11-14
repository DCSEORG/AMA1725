// Azure OpenAI resources for GenAI chat functionality
// Low-cost development deployment in UK South
// This is optional - controlled by includeChatUI parameter

param location string = 'uksouth'
param openAIAccountName string = 'openai-expenseapp-${uniqueString(resourceGroup().id)}'
param includeChatUI bool = false // Default to false - GenAI features not deployed
param webAppName string

// Cognitive Services OpenAI Account (only if includeChatUI is true)
resource openAI 'Microsoft.CognitiveServices/accounts@2023-05-01' = if (includeChatUI) {
  name: openAIAccountName
  location: location
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: openAIAccountName
    publicNetworkAccess: 'Enabled'
  }
}

// GPT-3.5-Turbo deployment (only if includeChatUI is true)
resource gpt35TurboDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = if (includeChatUI) {
  parent: openAI
  name: 'gpt-35-turbo'
  sku: {
    name: 'Standard'
    capacity: 10 // Low capacity for dev
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-35-turbo'
      version: '0301'
    }
  }
}

// Optional: Azure AI Search for RAG pattern (only if includeChatUI is true)
resource searchService 'Microsoft.Search/searchServices@2023-11-01' = if (includeChatUI) {
  name: 'search-expenseapp-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'basic' // Lowest cost tier
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
  }
}

// Update Web App settings with GenAI configuration (only if includeChatUI is true)
resource webApp 'Microsoft.Web/sites@2023-01-01' existing = {
  name: webAppName
}

resource webAppSettings 'Microsoft.Web/sites/config@2023-01-01' = if (includeChatUI) {
  parent: webApp
  name: 'appsettings'
  properties: {
    ASPNETCORE_ENVIRONMENT: 'Development'
    UseDummyData: 'true'
    IncludeChatUI: string(includeChatUI)
    AzureOpenAI__Endpoint: includeChatUI ? openAI.properties.endpoint : ''
    AzureOpenAI__DeploymentName: includeChatUI ? 'gpt-35-turbo' : ''
    AzureOpenAI__ApiKey: includeChatUI ? openAI.listKeys().key1 : ''
    AzureSearch__Endpoint: includeChatUI ? 'https://${searchService.name}.search.windows.net' : ''
    AzureSearch__ApiKey: includeChatUI ? searchService.listAdminKeys().primaryKey : ''
  }
}

output openAIEndpoint string = includeChatUI ? openAI.properties.endpoint : ''
output openAIAccountName string = includeChatUI ? openAI.name : ''
output searchServiceName string = includeChatUI ? searchService.name : ''
