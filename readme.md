# simula-plus-api

Sistema de simulados para exames e entrevistas.

## Pré-requisitos
- Visual Studio Community 2022.

---

## Rebaselining migrations

- 1 - Escolha seu banco de dados no `appsettings.Development.json`.
- 2 - Remova todas as migrations existentes (caso existam) dentro da pasta `Database/Migrations`.
- 2 - Crie a migration inicial com o comando abaixo.

```bash

# cria sua migration
Add-Migration MigrationInicial -OutputDir "Database/Migrations"
```

- 3 - Rebaselining migrations

Coloque um return no início do método `Up` da migration criada acima e rode o comando abaixo. 


- 4 - Atulize o banco de dados com o comando abaixo:
```bash
# aplica a migration
Update-Database
```



---
## Ambientes 
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


## 🗄️ Bancos de daodos - Colinha docker

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
