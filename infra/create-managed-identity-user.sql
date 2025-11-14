-- Script to create database user for App Service Managed Identity
-- Run this after deploying the infrastructure
-- Replace <webapp-name> with your actual Web App name

-- Connect to ExpenseDB as the SQL admin
-- Then run:

CREATE USER [<webapp-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<webapp-name>];
ALTER ROLE db_datawriter ADD MEMBER [<webapp-name>];
ALTER ROLE db_ddladmin ADD MEMBER [<webapp-name>];
GO
