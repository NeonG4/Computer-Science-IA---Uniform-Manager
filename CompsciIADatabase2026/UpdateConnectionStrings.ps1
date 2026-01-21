# =============================================
# Update Connection Strings for Azure SQL
# Run AFTER SetupAzureDatabase.ps1
# =============================================

param(
    [Parameter(Mandatory=$false)]
    [string]$ConnectionStringFile = "AzureConnectionString.txt"
)

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Updating Connection Strings" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Read connection string
if (-not (Test-Path $ConnectionStringFile)) {
    Write-Host "ERROR: $ConnectionStringFile not found!" -ForegroundColor Red
    Write-Host "Please run SetupAzureDatabase.ps1 first." -ForegroundColor Yellow
    exit 1
}

$connectionString = Get-Content $ConnectionStringFile -Raw
$connectionString = $connectionString.Trim()

Write-Host "Using connection string from: $ConnectionStringFile" -ForegroundColor Green
Write-Host ""

# Update Azure Function local.settings.json
$functionSettingsPath = "..\CompsciAzureFunctionAPI2026\local.settings.json"
if (Test-Path $functionSettingsPath) {
    Write-Host "Updating Azure Function connection string..." -ForegroundColor Yellow
    
    $settings = Get-Content $functionSettingsPath | ConvertFrom-Json
    $settings.ConnectionStrings.SqlConnection = $connectionString
    $settings | ConvertTo-Json -Depth 10 | Set-Content $functionSettingsPath -Encoding UTF8
    
    Write-Host "? Azure Function updated" -ForegroundColor Green
} else {
    Write-Host "? Azure Function local.settings.json not found at: $functionSettingsPath" -ForegroundColor Yellow
}

# Create or update WinForms App.config
$winformsAppConfigPath = "..\Computer Science IA - Uniform Manager\App.config"
Write-Host ""
Write-Host "Creating WinForms App.config..." -ForegroundColor Yellow

$appConfigContent = @"
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="SqlConnection" 
         connectionString="$connectionString" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
"@

$appConfigContent | Set-Content $winformsAppConfigPath -Encoding UTF8
Write-Host "? WinForms App.config created/updated" -ForegroundColor Green

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Connection Strings Updated!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Files updated:" -ForegroundColor Cyan
Write-Host "  1. $functionSettingsPath" -ForegroundColor White
Write-Host "  2. $winformsAppConfigPath" -ForegroundColor White
Write-Host ""
Write-Host "Next step: Run the database setup script" -ForegroundColor Cyan
Write-Host "  .\RunAzureSetup.ps1" -ForegroundColor Yellow
Write-Host ""
