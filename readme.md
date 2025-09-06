# simula-plus-api

Sistema de simulados para exames e entrevistas.

## Pré-requisitos
- Visual Studio Community 2022.


# Database Migrations
```bash
# No Package Manager Console, execute os comandos
Add-Migration NOME_SIGNIFICATIVO

Update-Database
```
# Ambientes 
- [PROD](https://mock-exams.pegasus-soft.com.br//swagger)


# colinha bash

```bash

# restaurar dependências
dotnet restore ./src/SalveElas.sln

# build
dotnet build .\src\SalveElas.Api\SalveElas.Api.csproj --verbosity minimal

# rodar o app com hot reload
dotnet watch --project ./ShareBook/ShareBook.Api/ShareBook.Api.csproj

# rodar os testes
dotnet test ./ShareBook/ShareBook.Test.Unit/ShareBook.Test.Unit.csproj

# clean
dotnet clean .\src\SalveElas.Api\SalveElas.Api.csproj --verbosity quiet

```


# Como usar o SQL SERVER com docker?
```bash

# 1 - rode o sql server via docker
docker run --name my-sql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=LpVgt4fLMZbg7kcp" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest 

# 2 - Atualize a string de conexão no appsettings.development
# "Server=localhost;Database=DEVinSales;User=sa;Password=LpVgt4fLMZbg7kcp"

```

# Build e Run com Docker!
```bash
# Build da imagem
docker build -t simula-plus-api -f devops/Dockerfile .

# Run com environment Development
docker run -d -p 8000:8080 -e ASPNETCORE_ENVIRONMENT=Development --name simula-plus-container simula-plus-api
```
