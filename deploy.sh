#!/bin/bash

# Expense Management System - Deployment Script
# This script deploys all Azure infrastructure and application code
# Prerequisites: Azure CLI installed and logged in (az login)

set -e  # Exit on any error

# Configuration
RESOURCE_GROUP="rg-expenseapp-dev"
LOCATION="uksouth"
INCLUDE_CHAT_UI="false"  # Set to "true" to deploy GenAI chat features
SQL_ADMIN_PASSWORD="P@ssw0rd$(date +%s)Secure!"  # Generate unique password

echo "=========================================="
echo "Expense Management System Deployment"
echo "=========================================="
echo ""
echo "Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  Include Chat UI: $INCLUDE_CHAT_UI"
echo ""

# Step 1: Create Resource Group
echo "Step 1: Creating resource group..."
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output table

# Step 2: Deploy infrastructure using Bicep
echo ""
echo "Step 2: Deploying Azure infrastructure (App Service, SQL, OpenAI)..."
DEPLOYMENT_OUTPUT=$(az deployment group create \
  --resource-group "$RESOURCE_GROUP" \
  --template-file ./infra/main.bicep \
  --parameters location="$LOCATION" \
               includeChatUI="$INCLUDE_CHAT_UI" \
               sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
  --output json)

# Extract outputs
WEB_APP_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.webAppName.value')
WEB_APP_URL=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.webAppUrl.value')
SQL_SERVER_FQDN=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.sqlServerFqdn.value')
DATABASE_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.databaseName.value')

echo ""
echo "Infrastructure deployed successfully!"
echo "  Web App Name: $WEB_APP_NAME"
echo "  Web App URL: $WEB_APP_URL"
echo "  SQL Server: $SQL_SERVER_FQDN"
echo "  Database: $DATABASE_NAME"

# Step 3: Initialize database schema
echo ""
echo "Step 3: Initializing database schema..."
echo "NOTE: You may need to run the database schema manually:"
echo "  1. Connect to SQL Server: $SQL_SERVER_FQDN"
echo "  2. Run: Database-Schema/database_schema.sql"
echo "  3. Run: infra/create-managed-identity-user.sql (replace <webapp-name> with $WEB_APP_NAME)"

# Step 4: Deploy application code
if [ -f "./app.zip" ]; then
  echo ""
  echo "Step 4: Deploying application code..."
  az webapp deploy \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP_NAME" \
    --src-path ./app.zip \
    --type zip \
    --output table
  
  echo ""
  echo "Application deployed successfully!"
else
  echo ""
  echo "Step 4: Skipped - app.zip not found. Build the application first."
fi

# Final output
echo ""
echo "=========================================="
echo "Deployment Complete!"
echo "=========================================="
echo ""
echo "Web Application URL: $WEB_APP_URL/Index"
echo ""
echo "IMPORTANT: Navigate to $WEB_APP_URL/Index (not just root URL)"
echo ""
echo "Next steps:"
echo "  1. Initialize the database schema using the SQL scripts"
echo "  2. Test the application at $WEB_APP_URL/Index"
if [ "$INCLUDE_CHAT_UI" == "true" ]; then
  echo "  3. Access the GenAI Chat UI at $WEB_APP_URL/Chat"
fi
echo ""
echo "To deploy WITH GenAI Chat UI features, edit this script and set:"
echo "  INCLUDE_CHAT_UI=\"true\""
echo ""
