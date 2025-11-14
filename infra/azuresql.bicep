// Azure SQL Database deployment for Expense Management System
// Low-cost development SKU in UK South

param location string = 'uksouth'
param sqlServerName string = 'sql-expenseapp-${uniqueString(resourceGroup().id)}'
param databaseName string = 'ExpenseDB'
param administratorLogin string = 'sqladmin'
@secure()
param administratorLoginPassword string
param webAppPrincipalId string = ''

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    version: '12.0'
    publicNetworkAccess: 'Enabled'
  }
}

// Firewall rule to allow Azure services
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// SQL Database with Basic SKU (lowest cost)
resource database 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648 // 2GB
  }
}

// Enable Azure AD Authentication and create database user for managed identity
// This requires manual execution of SQL script after deployment
// See: create-managed-identity-user.sql

output sqlServerName string = sqlServer.name
output databaseName string = database.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
