FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/Services/Notification/Notification.API/Notification.API.csproj Services/Notification/Notification.API/
RUN dotnet restore Services/Notification/Notification.API/Notification.API.csproj

COPY src/ .
RUN dotnet publish Services/Notification/Notification.API/Notification.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Notification.API.dll"]
