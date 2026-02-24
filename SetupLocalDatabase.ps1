# =============================================
# Setup Local SQL Database
# Simple script to create local database for development
# =============================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Local Database Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$databaseName = "DatabasestorageIA"
$sqlFile = "CompsciIADatabase2026\00_CompleteSetup.sql"

Write-Host "This script will create a local database for development." -ForegroundColor Yellow
Write-Host ""
Write-Host "Database: $databaseName" -ForegroundColor White
Write-Host "Instance: (localdb)\MSSQLLocalDB" -ForegroundColor White
Write-Host ""

# Check if SQL file exists
if (-not (Test-Path $sqlFile)) {
    Write-Host "ERROR: SQL setup file not found!" -ForegroundColor Red
    Write-Host "Expected: $sqlFile" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please ensure you're running this from the repository root." -ForegroundColor Yellow
    exit 1
}

Write-Host "Step 1: Creating database..." -ForegroundColor Green
Write-Host ""

# Create database using sqlcmd
try {
    # First create the database if it doesn't exist
    $createDbSql = @"
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName')
BEGIN
    CREATE DATABASE [$databaseName]
END
GO
"@

    $createDbSql | sqlcmd -S "(localdb)\MSSQLLocalDB" -b
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Database created successfully" -ForegroundColor Green
    } else {
        Write-Host "? Database might already exist (this is OK)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "? Could not create database (it might already exist)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 2: Creating tables and sample data..." -ForegroundColor Green
Write-Host ""

# Run the setup script
try {
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d $databaseName -i $sqlFile -b
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "=========================================" -ForegroundColor Green
        Write-Host "Local Database Setup Complete!" -ForegroundColor Green
        Write-Host "=========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Connection Details:" -ForegroundColor Cyan
        Write-Host "  Server: (localdb)\MSSQLLocalDB" -ForegroundColor White
        Write-Host "  Database: $databaseName" -ForegroundColor White
        Write-Host "  Authentication: Windows Authentication" -ForegroundColor White
        Write-Host ""
        Write-Host "Next Steps:" -ForegroundColor Cyan
        Write-Host "  1. Start the Azure Function (F5 in Visual Studio)" -ForegroundColor White
        Write-Host "  2. Run your WinForms application" -ForegroundColor White
        Write-Host "  3. Create an account and test!" -ForegroundColor White
        Write-Host ""
    } else {
        throw "SQL script execution failed"
    }
} catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Error Setting Up Database" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "The automated setup failed. Please try manually:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: SQL Server Management Studio (SSMS)" -ForegroundColor Cyan
    Write-Host "  1. Open SSMS" -ForegroundColor White
    Write-Host "  2. Connect to: (localdb)\MSSQLLocalDB" -ForegroundColor White
    Write-Host "  3. Open file: $sqlFile" -ForegroundColor White
    Write-Host "  4. Execute (F5)" -ForegroundColor White
    Write-Host ""
    Write-Host "Option 2: Visual Studio" -ForegroundColor Cyan
    Write-Host "  1. View ? SQL Server Object Explorer" -ForegroundColor White
    Write-Host "  2. Connect to (localdb)\MSSQLLocalDB" -ForegroundColor White
    Write-Host "  3. Right-click database ? New Query" -ForegroundColor White
    Write-Host "  4. Copy/paste contents of: $sqlFile" -ForegroundColor White
    Write-Host "  5. Execute" -ForegroundColor White
    Write-Host ""
    
    exit 1
}
