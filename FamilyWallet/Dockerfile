# 1. Възстановяване на зависимости
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore FamilyWallet.sln

# 2. Компилация
RUN dotnet publish FamilyWallet.API/FamilyWallet.API.csproj -c Release -o /app/publish

# 3. Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5095
ENTRYPOINT ["dotnet", "FamilyWallet.API.dll"]
