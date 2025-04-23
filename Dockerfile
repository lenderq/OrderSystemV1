#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderSystemV1/OrderSystemV1.csproj", "OrderSystemV1/"]
RUN dotnet restore "OrderSystemV1/OrderSystemV1.csproj"
COPY . .
WORKDIR "/src/OrderSystemV1"
RUN dotnet publish "./OrderSystemV1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderSystemV1.dll"]