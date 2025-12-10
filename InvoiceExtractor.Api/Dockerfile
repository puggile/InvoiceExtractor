# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia e ripristina i pacchetti (layer caching)
COPY ["InvoiceExtractor.Api.csproj", "./"]
RUN dotnet restore "InvoiceExtractor.Api.csproj"

# Copia tutto il resto e compila
COPY . .
RUN dotnet publish "InvoiceExtractor.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InvoiceExtractor.Api.dll"]