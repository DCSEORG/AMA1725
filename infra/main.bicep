// Main deployment file for Expense Management System
// Orchestrates all Azure resources

targetScope = 'resourceGroup'

param location string = 'uksouth'
param includeChatUI bool = false // Default: false - deploy without GenAI features
@secure()
param sqlAdminPassword string

// Deploy App Service
module appService 'appservice.bicep' = {
  name: 'appServiceDeployment'
  params: {
    location: location
  }
}

// Deploy Azure SQL Database
module sqlDatabase 'azuresql.bicep' = {
  name: 'sqlDatabaseDeployment'
  params: {
    location: location
    administratorLoginPassword: sqlAdminPassword
    webAppPrincipalId: appService.outputs.webAppPrincipalId
  }
}

// Deploy Azure OpenAI (optional, based on includeChatUI parameter)
module openAI 'azureopenai.bicep' = {
  name: 'openAIDeployment'
  params: {
    location: location
    includeChatUI: includeChatUI
    webAppName: appService.outputs.webAppName
  }
  dependsOn: [
    appService
  ]
}

output webAppUrl string = appService.outputs.webAppUrl
output webAppName string = appService.outputs.webAppName
output sqlServerFqdn string = sqlDatabase.outputs.sqlServerFqdn
output databaseName string = sqlDatabase.outputs.databaseName
output openAIEndpoint string = includeChatUI ? openAI.outputs.openAIEndpoint : 'Not deployed - includeChatUI is false'
