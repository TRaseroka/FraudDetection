FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FraudDetection.Api/FraudDetection.Api.csproj", "FraudDetection.Api/"]
COPY ["FraudDetection.Application/FraudDetection.Application.csproj", "FraudDetection.Application/"]
COPY ["FraudDetection.Domain/FraudDetection.Domain.csproj", "FraudDetection.Domain/"]
COPY ["FraudDetection.Infrastructure/FraudDetection.Infrastructure.csproj", "FraudDetection.Infrastructure/"]

RUN dotnet restore "FraudDetection.Api/FraudDetection.Api.csproj"

COPY . .

WORKDIR "/src/FraudDetection.Api"
RUN dotnet publish "FraudDetection.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FraudDetection.Api.dll"]