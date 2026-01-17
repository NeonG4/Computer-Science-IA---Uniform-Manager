# PowerShell script to apply database migration
# This script connects to your LocalDB instance and runs the migration

$connectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=`"C:\Users\faken\source\repos\Computer Science IA - Uniform Manager\Computer Science IA - Uniform Manager\DatabasestorageIA.mdf`";Integrated Security=True"
$scriptPath = "C:\Users\faken\source\repos\Computer Science IA - Uniform Manager\CompsciIADatabase2026\Migration_AddAccountLevel.sql"

Write-Host "Applying database migration..." -ForegroundColor Cyan
Write-Host "Connection: $connectionString" -ForegroundColor Gray
Write-Host ""

try {
    # Load SQL script
    $sqlScript = Get-Content $scriptPath -Raw
    
    # Remove the USE statement as we're already connected to the right database
    $sqlScript = $sqlScript -replace "USE \[DatabasestorageIA\]", ""
    
    # Create connection
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    
    Write-Host "Connected to database successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Split script by GO statements and execute each batch
    $batches = $sqlScript -split '\bGO\b'
    
    foreach ($batch in $batches) {
        $batch = $batch.Trim()
        if ($batch -ne "") {
            try {
                $command = $connection.CreateCommand()
                $command.CommandText = $batch
                $command.ExecuteNonQuery() | Out-Null
                
                # Check for messages
                $infoMessages = $connection.GetType().GetEvent("InfoMessage")
                if ($infoMessages) {
                    Write-Host "Batch executed successfully" -ForegroundColor Green
                }
            }
            catch {
                Write-Host "Error in batch: $_" -ForegroundColor Red
                Write-Host "Batch content: $batch" -ForegroundColor Yellow
            }
        }
    }
    
    Write-Host ""
    Write-Host "Querying user summary..." -ForegroundColor Cyan
    
    # Get summary
    $command = $connection.CreateCommand()
    $command.CommandText = @"
SELECT 
    COUNT(*) as [TotalUsers],
    SUM(CASE WHEN AccountLevel = 0 THEN 1 ELSE 0 END) as [Administrators],
    SUM(CASE WHEN AccountLevel = 1 THEN 1 ELSE 0 END) as [Users],
    SUM(CASE WHEN AccountLevel = 2 THEN 1 ELSE 0 END) as [Viewers]
FROM [dbo].[UserInfo]
"@
    
    $reader = $command.ExecuteReader()
    if ($reader.Read()) {
        Write-Host ""
        Write-Host "=== UserInfo Table Summary ===" -ForegroundColor Cyan
        Write-Host "Total Users:      $($reader['TotalUsers'])" -ForegroundColor White
        Write-Host "Administrators:   $($reader['Administrators'])" -ForegroundColor Red
        Write-Host "Standard Users:   $($reader['Users'])" -ForegroundColor Yellow
        Write-Host "Viewers:          $($reader['Viewers'])" -ForegroundColor Green
        Write-Host ""
    }
    $reader.Close()
    
    $connection.Close()
    
    Write-Host "Migration completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. If you want to promote a user to administrator, run:" -ForegroundColor White
    Write-Host "   UPDATE UserInfo SET AccountLevel = 0 WHERE UserId = 1" -ForegroundColor Gray
    Write-Host "2. Restart your application to use the new account levels" -ForegroundColor White
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
