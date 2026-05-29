FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/Services/Inventory/Inventory.API/Inventory.API.csproj Services/Inventory/Inventory.API/
RUN dotnet restore Services/Inventory/Inventory.API/Inventory.API.csproj

COPY src/ .
RUN dotnet publish Services/Inventory/Inventory.API/Inventory.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Inventory.API.dll"]
