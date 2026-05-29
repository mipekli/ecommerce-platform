FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Caching/Shared.Caching.csproj BuildingBlocks/Shared.Caching/
COPY src/Services/Product/Product.API/Product.API.csproj Services/Product/Product.API/
RUN dotnet restore Services/Product/Product.API/Product.API.csproj

COPY src/ .
RUN dotnet publish Services/Product/Product.API/Product.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Product.API.dll"]
