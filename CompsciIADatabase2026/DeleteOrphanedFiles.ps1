# =============================================
# Delete Orphaned Database Files
# Run this as Administrator in PowerShell
# =============================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Deleting Orphaned Database Files" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$filesToDelete = @(
    "C:\Users\1117078\DatabasestorageIA.mdf",
    "C:\Users\1117078\DatabasestorageIA_log.ldf",
    "C:\Users\1117078\DatabasestorageIA_NEW.mdf",
    "C:\Users\1117078\DatabasestorageIA_NEW_log.ldf"
)

foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        Write-Host "Deleting: $file" -ForegroundColor Yellow
        try {
            Remove-Item $file -Force
            Write-Host "  ? Deleted" -ForegroundColor Green
        }
        catch {
            Write-Host "  ? Could not delete (file may be in use)" -ForegroundColor Red
            Write-Host "  Try closing SQL Management Studio and Azure Function" -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "Not found: $file" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Cleanup Complete!" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Run SimpleReset.sql in SQL Management Studio" -ForegroundColor White
Write-Host "  2. Restart Azure Function (F5)" -ForegroundColor White
Write-Host "  3. Try your app again" -ForegroundColor White
Write-Host ""
