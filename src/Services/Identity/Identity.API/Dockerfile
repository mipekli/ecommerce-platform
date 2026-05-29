FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/BuildingBlocks/Shared.Caching/Shared.Caching.csproj BuildingBlocks/Shared.Caching/
COPY src/Services/Identity/Identity.Domain/Identity.Domain.csproj Services/Identity/Identity.Domain/
COPY src/Services/Identity/Identity.Application/Identity.Application.csproj Services/Identity/Identity.Application/
COPY src/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj Services/Identity/Identity.Infrastructure/
COPY src/Services/Identity/Identity.API/Identity.API.csproj Services/Identity/Identity.API/
RUN dotnet restore Services/Identity/Identity.API/Identity.API.csproj

COPY src/ .
RUN dotnet publish Services/Identity/Identity.API/Identity.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Identity.API.dll"]
