# =============================================
# Complete Azure Migration Script
# Run this to migrate database and update configurations
# =============================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Uniform Manager - Azure Migration" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Azure CLI is installed
try {
    $azVersion = az version | ConvertFrom-Json
    Write-Host "? Azure CLI detected: $($azVersion.'azure-cli')" -ForegroundColor Green
} catch {
    Write-Host "? Azure CLI not found!" -ForegroundColor Red
    Write-Host "Please install from: https://docs.microsoft.com/cli/azure/install-azure-cli" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "This script will:" -ForegroundColor Yellow
Write-Host "  1. Create Azure SQL Database" -ForegroundColor White
Write-Host "  2. Deploy database schema" -ForegroundColor White
Write-Host "  3. Update local.settings.json" -ForegroundColor White
Write-Host "  4. Prepare for Azure Function deployment" -ForegroundColor White
Write-Host ""

$continue = Read-Host "Continue? (Y/N)"
if ($continue -ne "Y" -and $continue -ne "y") {
    Write-Host "Migration cancelled." -ForegroundColor Yellow
    exit 0
}

# Variables - CUSTOMIZE THESE
$resourceGroup = "UniformManagerRG"
$location = "eastus"
$serverName = "uniformmanager-sql-$(Get-Random -Maximum 9999)"
$databaseName = "DatabasestorageIA"
$adminLogin = "sqladmin"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Step 1: Azure SQL Database Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Server name: $serverName" -ForegroundColor Yellow

# Get password
$adminPassword = Read-Host "Enter SQL Admin Password (min 12 chars, mix of upper, lower, numbers, symbols)" -AsSecureString
$adminPasswordText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($adminPassword))

# Login to Azure
Write-Host ""
Write-Host "Logging in to Azure..." -ForegroundColor Green
az login

# Create Resource Group
Write-Host ""
Write-Host "Creating Resource Group..." -ForegroundColor Green
az group create --name $resourceGroup --location $location

# Create SQL Server
Write-Host ""
Write-Host "Creating SQL Server (this may take a few minutes)..." -ForegroundColor Green
az sql server create `
    --name $serverName `
    --resource-group $resourceGroup `
    --location $location `
    --admin-user $adminLogin `
    --admin-password $adminPasswordText

# Create Database
Write-Host ""
Write-Host "Creating Database (Basic tier)..." -ForegroundColor Green
az sql db create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name $databaseName `
    --service-objective Basic `
    --backup-storage-redundancy Local

# Configure Firewall
Write-Host ""
Write-Host "Configuring Firewall..." -ForegroundColor Green

az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name AllowAzureServices `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

$myIp = (Invoke-WebRequest -Uri "https://api.ipify.org").Content
Write-Host "Your IP: $myIp" -ForegroundColor Yellow

az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name AllowMyIP `
    --start-ip-address $myIp `
    --end-ip-address $myIp

# Generate connection string
$connectionString = "Server=tcp:$serverName.database.windows.net,1433;Initial Catalog=$databaseName;Persist Security Info=False;User ID=$adminLogin;Password=$adminPasswordText;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Azure SQL Database Created!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Connection Details:" -ForegroundColor Cyan
Write-Host "  Server: $serverName.database.windows.net" -ForegroundColor White
Write-Host "  Database: $databaseName" -ForegroundColor White
Write-Host "  Username: $adminLogin" -ForegroundColor White
Write-Host ""

# Save connection string
$connectionString | Out-File -FilePath "AzureConnectionString.txt" -Encoding UTF8
Write-Host "? Connection string saved to: AzureConnectionString.txt" -ForegroundColor Green

# Update local.settings.json
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Step 2: Update Configurations" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$functionSettingsPath = "CompsciAzureFunctionAPI2026\local.settings.json"
if (Test-Path $functionSettingsPath) {
    Write-Host "Updating Azure Function connection string..." -ForegroundColor Yellow
    
    $settings = Get-Content $functionSettingsPath | ConvertFrom-Json
    $settings.ConnectionStrings.SqlConnection = $connectionString
    $settings | ConvertTo-Json -Depth 10 | Set-Content $functionSettingsPath -Encoding UTF8
    
    Write-Host "? local.settings.json updated" -ForegroundColor Green
} else {
    Write-Host "? local.settings.json not found" -ForegroundColor Red
}

# Deploy database schema
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Step 3: Deploy Database Schema" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$sqlFile = "CompsciIADatabase2026\00_CompleteSetup.sql"
if (Test-Path $sqlFile) {
    Write-Host "Deploying database schema..." -ForegroundColor Yellow
    Write-Host "This requires sqlcmd to be installed." -ForegroundColor Yellow
    Write-Host ""
    
    $deploySql = Read-Host "Deploy schema now? (Y/N)"
    if ($deploySql -eq "Y" -or $deploySql -eq "y") {
        try {
            sqlcmd -S "$serverName.database.windows.net" `
                   -d "$databaseName" `
                   -U "$adminLogin" `
                   -P "$adminPasswordText" `
                   -i "$sqlFile"
            Write-Host "? Database schema deployed successfully" -ForegroundColor Green
        } catch {
            Write-Host "? Error deploying schema. You can do this manually later." -ForegroundColor Red
            Write-Host "Use SQL Server Management Studio or Azure Data Studio to run: $sqlFile" -ForegroundColor Yellow
        }
    } else {
        Write-Host "Skipped. You can deploy manually later." -ForegroundColor Yellow
    }
} else {
    Write-Host "? SQL setup file not found: $sqlFile" -ForegroundColor Yellow
}

# Create Azure Function deployment info
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Step 4: Azure Function Deployment" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$functionAppName = "uniformmanager-api-$(Get-Random -Maximum 9999)"
$storageAccount = "uniformstorage$(Get-Random -Maximum 9999)"

Write-Host "Recommended Function App Name: $functionAppName" -ForegroundColor Yellow
Write-Host ""
Write-Host "To deploy Azure Function:" -ForegroundColor Cyan
Write-Host "  Option 1 - Visual Studio:" -ForegroundColor White
Write-Host "    1. Right-click CompsciAzureFunctionAPI2026 project" -ForegroundColor Gray
Write-Host "    2. Select 'Publish'" -ForegroundColor Gray
Write-Host "    3. Choose Azure > Azure Function App" -ForegroundColor Gray
Write-Host "    4. Create new with name: $functionAppName" -ForegroundColor Gray
Write-Host "    5. Use Resource Group: $resourceGroup" -ForegroundColor Gray
Write-Host ""
Write-Host "  Option 2 - Azure CLI:" -ForegroundColor White
Write-Host "    Run the following commands:" -ForegroundColor Gray
Write-Host ""

$deployCommands = @"
# Create storage account
az storage account create ``
  --name $storageAccount ``
  --resource-group $resourceGroup ``
  --location $location ``
  --sku Standard_LRS

# Create Function App
az functionapp create ``
  --resource-group $resourceGroup ``
  --consumption-plan-location $location ``
  --runtime dotnet-isolated ``
  --runtime-version 8 ``
  --functions-version 4 ``
  --name $functionAppName ``
  --storage-account $storageAccount

# Set connection string
az functionapp config connection-string set ``
  --name $functionAppName ``
  --resource-group $resourceGroup ``
  --connection-string-type SQLAzure ``
  --settings SqlConnection="$connectionString"

# Deploy (from project directory)
cd CompsciAzureFunctionAPI2026
func azure functionapp publish $functionAppName
"@

$deployCommands | Out-File -FilePath "DeployAzureFunction.ps1" -Encoding UTF8
Write-Host $deployCommands -ForegroundColor DarkGray
Write-Host ""
Write-Host "? Commands saved to: DeployAzureFunction.ps1" -ForegroundColor Green

# Summary
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Migration Setup Complete!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  ? Azure SQL Database created" -ForegroundColor Green
Write-Host "  ? Connection string saved" -ForegroundColor Green
Write-Host "  ? local.settings.json updated" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Deploy database schema (if not done above)" -ForegroundColor White
Write-Host "  2. Deploy Azure Function using Visual Studio or CLI" -ForegroundColor White
Write-Host "  3. Update App.config with Azure Function URL:" -ForegroundColor White
Write-Host "     https://$functionAppName.azurewebsites.net/api" -ForegroundColor Yellow
Write-Host "  4. Test the application" -ForegroundColor White
Write-Host ""
Write-Host "Files created:" -ForegroundColor Cyan
Write-Host "  - AzureConnectionString.txt" -ForegroundColor White
Write-Host "  - DeployAzureFunction.ps1" -ForegroundColor White
Write-Host ""
Write-Host "See AZURE_MIGRATION_GUIDE.md for detailed instructions" -ForegroundColor Yellow
Write-Host ""
