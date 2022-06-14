#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["maze-text-game/maze-text-game.csproj", "maze-text-game/"]
RUN dotnet restore "maze-text-game/maze-text-game.csproj"
COPY . .
WORKDIR "/src/maze-text-game"
RUN dotnet build "maze-text-game.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "maze-text-game.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "maze-text-game.dll"]