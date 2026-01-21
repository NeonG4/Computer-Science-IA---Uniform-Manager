# =============================================
# Run Complete Database Setup
# =============================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Uniform Manager - Database Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ServerInstance = "(localdb)\MSSQLLocalDB"
$DatabaseName = "DatabasestorageIA"
$SqlFile = Join-Path $PSScriptRoot "00_CompleteSetup.sql"

# Check if SQL file exists
if (-not (Test-Path $SqlFile)) {
    Write-Host "ERROR: SQL file not found: $SqlFile" -ForegroundColor Red
    exit 1
}

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Server: $ServerInstance" -ForegroundColor Gray
Write-Host "  Database: $DatabaseName" -ForegroundColor Gray
Write-Host "  Script: $(Split-Path $SqlFile -Leaf)" -ForegroundColor Gray
Write-Host ""

# Try to connect to database
Write-Host "Testing database connection..." -ForegroundColor Yellow
try {
    $testQuery = "SELECT DB_NAME() AS DatabaseName, @@VERSION AS Version"
    $result = Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $testQuery -ErrorAction Stop
    Write-Host "? Connected to: $($result.DatabaseName)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "? Failed to connect to database" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please ensure:" -ForegroundColor Yellow
    Write-Host "  1. LocalDB is running" -ForegroundColor Gray
    Write-Host "  2. Database 'DatabasestorageIA' exists" -ForegroundColor Gray
    Write-Host "  3. You have proper permissions" -ForegroundColor Gray
    exit 1
}

# Run the setup script
Write-Host "Running database setup script..." -ForegroundColor Yellow
Write-Host ""

try {
    # Read and execute the SQL file
    $sqlContent = Get-Content $SqlFile -Raw
    
    # Split by GO statements and execute each batch
    $batches = $sqlContent -split '\r?\nGO\r?\n'
    
    foreach ($batch in $batches) {
        if ($batch.Trim() -ne '') {
            Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $batch -ErrorAction Stop -Verbose
        }
    }
    
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "? Database setup completed successfully!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "? Database setup failed" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# Display final table counts
Write-Host "Verifying tables..." -ForegroundColor Yellow
try {
    $verifyQuery = @"
SELECT 
    t.name AS TableName,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
    AND t.name IN ('UserInfo', 'Students', 'Uniforms')
ORDER BY t.name
"@
    
    $tables = Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $verifyQuery
    
    Write-Host ""
    Write-Host "Tables created:" -ForegroundColor Cyan
    foreach ($table in $tables) {
        Write-Host "  • $($table.TableName): $($table.RowCount) rows" -ForegroundColor Gray
    }
    Write-Host ""
    
} catch {
    Write-Host "Warning: Could not verify tables" -ForegroundColor Yellow
}

Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Press F5 to start the Azure Function" -ForegroundColor Gray
Write-Host "  2. Test the APIs using the documentation" -ForegroundColor Gray
Write-Host "  3. Create your first admin account" -ForegroundColor Gray
Write-Host ""
Write-Host "Done! ??" -ForegroundColor Green
Write-Host ""
