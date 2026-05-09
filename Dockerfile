# ====================== Stage 1: Build ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MinimalAPI.csproj", "./"]
RUN dotnet restore "MinimalAPI.csproj"

COPY . .
RUN dotnet publish "MinimalAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ====================== Stage 2: Runtime ======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MinimalAPI.dll"]