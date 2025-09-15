param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet("postgres", "sqlserver", "sqlite")]
    [string]$Provider,
    
    [Parameter(Mandatory=$true, Position=1)]
    [string]$MigrationName
)

Write-Host "Criando migration '$MigrationName' para $Provider..." -ForegroundColor Green

# Atualiza o appsettings.json para o provider correto
$appsettingsPath = "src/Api/appsettings.json"
$appsettingsContent = Get-Content $appsettingsPath | ConvertFrom-Json

switch ($Provider) {
    "postgres" { 
        $appsettingsContent.DatabaseProvider = "Postgres"
        $outputDir = "Postgres"
    }
    "sqlserver" { 
        $appsettingsContent.DatabaseProvider = "SqlServer" 
        $outputDir = "SqlServer"
    }
    "sqlite" { 
        $appsettingsContent.DatabaseProvider = "Sqlite"
        $outputDir = "Sqlite"
    }
}

# Salva o appsettings.json atualizado
$appsettingsContent | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath

Write-Host "1. DatabaseProvider configurado para: $($appsettingsContent.DatabaseProvider)" -ForegroundColor Yellow

Write-Host "2. Fazendo build do projeto..." -ForegroundColor Yellow
dotnet build src/Api/Api.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "3. Criando migration no projeto Infra..." -ForegroundColor Yellow
    dotnet ef migrations add $MigrationName -p src/Infra -s src/Api --output-dir "Database/Migrations/$outputDir"
} else {
    Write-Host "Erro no build. Migration cancelada." -ForegroundColor Red
    exit 1
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$MigrationName' criada com sucesso em Database/Migrations/$outputDir!" -ForegroundColor Green
} else {
    Write-Host "Erro ao criar migration." -ForegroundColor Red
    exit 1
}