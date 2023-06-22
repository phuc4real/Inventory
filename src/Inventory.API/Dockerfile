#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Inventory.API/Inventory.API.csproj", "Inventory.API/"]
COPY ["Inventory.Services/Inventory.Services.csproj", "Inventory.Services/"]
COPY ["Inventory.Repository/Inventory.Repository.csproj", "Inventory.Repository/"]
COPY ["Shared/Inventory.Core/Inventory.Core.csproj", "Shared/Inventory.Core/"]
RUN dotnet restore "Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inventory.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]