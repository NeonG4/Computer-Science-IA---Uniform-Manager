# =============================================
# Setup Azure SQL Database for Uniform Manager
# Run this script to create your cloud database
# =============================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Azure SQL Database Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Variables - CUSTOMIZE THESE
$resourceGroup = "UniformManagerRG"
$location = "eastus"  # Change to your preferred region
$serverName = "uniformmanager-server-$(Get-Random -Maximum 9999)"  # Must be globally unique
$databaseName = "DatabasestorageIA"
$adminLogin = "sqladmin"

Write-Host "Server name will be: $serverName" -ForegroundColor Yellow
$adminPassword = Read-Host "Enter SQL Admin Password (min 12 chars, mix of upper, lower, numbers, symbols)" -AsSecureString
$adminPasswordText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($adminPassword))

Write-Host ""
Write-Host "Step 1: Logging in to Azure..." -ForegroundColor Green
az login

Write-Host ""
Write-Host "Step 2: Creating Resource Group..." -ForegroundColor Green
az group create `
    --name $resourceGroup `
    --location $location

Write-Host ""
Write-Host "Step 3: Creating SQL Server..." -ForegroundColor Green
az sql server create `
    --name $serverName `
    --resource-group $resourceGroup `
    --location $location `
    --admin-user $adminLogin `
    --admin-password $adminPasswordText

Write-Host ""
Write-Host "Step 4: Creating Database (Basic tier)..." -ForegroundColor Green
az sql db create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name $databaseName `
    --service-objective Basic `
    --backup-storage-redundancy Local

Write-Host ""
Write-Host "Step 5: Configuring Firewall..." -ForegroundColor Green

# Allow Azure services
az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name AllowAzureServices `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

# Allow your current IP
$myIp = (Invoke-WebRequest -Uri "https://api.ipify.org").Content
Write-Host "Your IP: $myIp" -ForegroundColor Yellow

az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name AllowMyIP `
    --start-ip-address $myIp `
    --end-ip-address $myIp

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Azure SQL Database Created Successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Connection Details:" -ForegroundColor Cyan
Write-Host "  Server: $serverName.database.windows.net" -ForegroundColor White
Write-Host "  Database: $databaseName" -ForegroundColor White
Write-Host "  Username: $adminLogin" -ForegroundColor White
Write-Host "  Password: [the password you entered]" -ForegroundColor White
Write-Host ""

# Generate connection string
$connectionString = "Server=tcp:$serverName.database.windows.net,1433;Initial Catalog=$databaseName;Persist Security Info=False;User ID=$adminLogin;Password=$adminPasswordText;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

Write-Host "Connection String:" -ForegroundColor Cyan
Write-Host $connectionString -ForegroundColor Yellow
Write-Host ""

# Save to file
$connectionString | Out-File -FilePath "AzureConnectionString.txt" -Encoding UTF8
Write-Host "Connection string saved to: AzureConnectionString.txt" -ForegroundColor Green
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Run the database setup script:" -ForegroundColor White
Write-Host "   .\MigrateToAzure.ps1" -ForegroundColor Yellow
Write-Host ""
Write-Host "2. Update your application connection strings (done automatically)" -ForegroundColor White
Write-Host ""

# Create migration script with connection string
$migrationScript = @"
# Run the database setup on Azure
sqlcmd -S "$serverName.database.windows.net" ``
       -d "$databaseName" ``
       -U "$adminLogin" ``
       -P "$adminPasswordText" ``
       -i "00_CompleteSetup.sql"
"@

$migrationScript | Out-File -FilePath "MigrateToAzure.ps1" -Encoding UTF8
Write-Host "Migration script created: MigrateToAzure.ps1" -ForegroundColor Green
