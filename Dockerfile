FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore IptvManagement.sln
RUN dotnet publish src/IptvManagement.Api/IptvManagement.Api.csproj -c Release -o /app/publish --no-restore
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","IptvManagement.Api.dll"]
