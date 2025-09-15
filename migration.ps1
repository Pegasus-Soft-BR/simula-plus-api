param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$MigrationName
)

Write-Host "CRIANDO MIGRATIONS PARA TODOS OS PROVIDERS!" -ForegroundColor Cyan
Write-Host "Migration: '$MigrationName'" -ForegroundColor White

# Backup do appsettings original
$appsettingsPath = "src/Api/appsettings.Development.json"
$backupPath = "$appsettingsPath.backup"
Copy-Item $appsettingsPath $backupPath

# Providers para processar (Postgres primeiro para evitar conflitos)
$providers = @(
    @{ Name = "Postgres"; Output = "Postgres"; Display = "PostgreSQL" },
    @{ Name = "SqlServer"; Output = "SqlServer"; Display = "SQL Server" },
    @{ Name = "SQLite"; Output = "Sqlite"; Display = "SQLite" }
)

$successCount = 0
$errors = @()

try {
    # Build inicial
    Write-Host "`nBuilding projeto..." -ForegroundColor Yellow
    dotnet build src/Api/Api.csproj --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        throw "Erro no build inicial"
    }

    foreach ($provider in $providers) {
        Write-Host "`nProcessando $($provider.Display)..." -ForegroundColor Magenta
        
        try {
            # Atualiza appsettings para o provider atual
            $appsettingsContent = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
            $appsettingsContent.DatabaseProvider = $provider.Name
            $appsettingsContent | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath
            
            # Cria migration para este provider
            $migrationCommand = "dotnet ef migrations add $($MigrationName)$($provider.Name) -p src/Infra -s src/Api --output-dir `"Database/Migrations/$($provider.Output)`""
            Invoke-Expression $migrationCommand
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   OK $($provider.Display) - SUCESSO!" -ForegroundColor Green
                $successCount++
            } else {
                $errors += "ERRO $($provider.Display) - FALHOU!"
                Write-Host "   ERRO $($provider.Display) - FALHOU!" -ForegroundColor Red
            }
        }
        catch {
            $errors += "ERRO $($provider.Display) - ERRO: $($_.Exception.Message)"
            Write-Host "   ERRO $($provider.Display) - ERRO: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}
finally {
    # Restaura appsettings original
    Move-Item $backupPath $appsettingsPath -Force
}

# Resultado final
Write-Host "`n" + "="*60 -ForegroundColor Cyan
Write-Host "RESULTADO FINAL:" -ForegroundColor Cyan
Write-Host "Sucessos: $successCount/3" -ForegroundColor Green

if ($errors.Count -gt 0) {
    Write-Host "Erros:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host "   $_" -ForegroundColor Red }
}

if ($successCount -eq 3) {
    Write-Host "`nBIRLLL! MIGRATIONS CRIADAS PARA TODOS OS BANCOS!" -ForegroundColor Green
    Write-Host "Simula+ agora eh MAIS FODA que o achei-api!" -ForegroundColor Cyan
} elseif ($successCount -gt 0) {
    Write-Host "`nAlgumas migrations criadas com sucesso, outras falharam." -ForegroundColor Yellow
} else {
    Write-Host "`nNenhuma migration foi criada. Verifique os erros acima." -ForegroundColor Red
    exit 1
}

Write-Host "="*60 -ForegroundColor Cyan