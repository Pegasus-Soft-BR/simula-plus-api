# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files for restore
COPY src/Api/Api.csproj ./src/Api/
COPY src/Domain/Domain.csproj ./src/Domain/
COPY src/Helper/Helper.csproj ./src/Helper/
COPY src/Infra/Infra.csproj ./src/Infra/
COPY src/Service/Service.csproj ./src/Service/
COPY src/Tests/Tests.csproj ./src/Tests/

# Copy source code
COPY src/ ./src/

# Restore and build in one step to avoid cache issues
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish --verbosity normal

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Instala curl - necessário pro health check funcionar
RUN apt-get update && apt-get install -y curl

WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# Expose port
EXPOSE 8080

# acho que não precisa mais disso, pois já está no launchSettings.json
# ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=30s \
  CMD curl -fsS http://127.0.0.1:8080/api/Operations/Ping || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "Api.dll"]