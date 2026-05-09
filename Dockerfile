# ====================== Stage 1: Build ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Копируем csproj и восстанавливаем зависимости
COPY ["MinimalAPI.csproj", "./"]
RUN dotnet restore "MinimalAPI.csproj"
# Копируем весь исходный код и публикуем приложение
COPY . .
RUN dotnet publish "MinimalAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ====================== Stage 2: Runtime ======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
# Копируем опубликованное приложение
COPY --from=build /app/publish .
# Открываем порт
EXPOSE 8080
# Запуск приложения
ENTRYPOINT ["dotnet", "MinimalAPI.dll"]