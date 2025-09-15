# simula-plus-api

Sistema de simulados para exames e entrevistas.

## Pré-requisitos
- Visual Studio Community 2022.


## Estratégia de criação do banco de dados

O banco de dados é criado e atualizado automaticamente via **Migrations** na primeira execução.

### Como criar migrations

```powershell
# O script detecta automaticamente o DatabaseProvider do appsettings.Development.json
.\migration.ps1 NomeDaMigration

# Exemplo:
.\migration.ps1 AddNewColumn
```

### Suporte multi-database
- **SQL Server:** migrations em `Database/Migrations/SqlServer/`
- **PostgreSQL:** migrations em `Database/Migrations/Postgres/`  
- **SQLite:** migrations em `Database/Migrations/Sqlite/`

### Como funciona
1. **Development:** `Migrate()` aplica migrations + seed popula dados
2. **Production:** `Migrate()` aplica migrations apenas
3. **Zero configuração:** dev abre projeto e funciona automaticamente
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
