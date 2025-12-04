FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Dungeon.Api/Dungeon.Api.csproj", "Dungeon.Api/"]
COPY ["Dungeon.Application/Dungeon.Application.csproj", "Dungeon.Application/"]
COPY ["Dungeon.Infrastructure/Dungeon.Infrastructure.csproj", "Dungeon.Infrastructure/"]
RUN dotnet restore "Dungeon.Api/Dungeon.Api.csproj"
COPY . .
WORKDIR "/src/Dungeon.Api"
RUN dotnet build "Dungeon.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dungeon.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dungeon.Api.dll"]