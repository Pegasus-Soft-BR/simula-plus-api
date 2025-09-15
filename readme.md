# simula-plus-api

Sistema de simulados para exames e entrevistas.

## Pré-requisitos
- Visual Studio Community 2022.


## Migrations

```powershell
# PostgreSQL
.\migration.ps1 postgres NomeMigration

# SQL Server  
.\migration.ps1 sqlserver NomeMigration

# SQLite
.\migration.ps1 sqlite NomeMigration
```
# Ambientes 
- [PROD](https://mock-exams.pegasus-soft.com.br/swagger)


## 🔧 Colinha bash

```bash
# restaurar dependências
dotnet restore ./src/SimulaPlus.sln

# build
dotnet build ./src/Api/Api.csproj --verbosity minimal

# rodar o app com hot reload
dotnet watch --project ./src/Api/Api.csproj

# rodar os testes
dotnet test ./src/Tests/Tests.csproj

# clean
dotnet clean ./src/Api/Api.csproj --verbosity quiet
```


## 🗄️ Colinha docker

Usar um banco local é muito mais rápido e acelera o desenvolvimento. Escolha seu banco favorito e aproveite!

```powershell
# sql server
docker run --name my-sql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=LpVgt4fLMZbg7kcp" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# postgres
docker run --name my-postgres -p 5432:5432 -e POSTGRES_PASSWORD=goku123 -e PGDATA=/var/lib/postgresql-static/data -d postgres
```

## 🐳 Docker da Aplicação

```bash
# Build da imagem
docker build -f devops/Dockerfile -t simula-plus-api .

# Run com environment Development
docker run -d --name simula-plus-api-dev -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development simula-plus-api

# Ver logs
docker logs -f simula-plus-api-dev

# Parar e remover
docker stop simula-plus-api-dev && docker rm simula-plus-api-dev
```
