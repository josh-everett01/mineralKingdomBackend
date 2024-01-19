#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MineralKingdomApi/MineralKingdomApi.csproj", "MineralKingdomApi/"]
COPY ["MineralKingdomApi.Data/MineralKingdomApi.Data.csproj", "MineralKingdomApi.Data/"]
RUN dotnet restore "MineralKingdomApi/MineralKingdomApi.csproj"
COPY . .
WORKDIR "/src/MineralKingdomApi"
RUN dotnet build "MineralKingdomApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineralKingdomApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineralKingdomApi.dll"]
