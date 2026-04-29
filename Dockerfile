FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FAIR.API/FAIR.API.csproj", "src/FAIR.API/"]
COPY ["src/FAIR.Application/FAIR.Application.csproj", "src/FAIR.Application/"]
COPY ["src/FAIR.Domain/FAIR.Domain.csproj", "src/FAIR.Domain/"]
COPY ["src/FAIR.Infrastructure/FAIR.Infrastructure.csproj", "src/FAIR.Infrastructure/"]
RUN dotnet restore "src/FAIR.API/FAIR.API.csproj"

COPY src/ ./src/
WORKDIR /src/src/FAIR.API
RUN dotnet build "FAIR.API.csproj" -c Release -o /app/build
RUN dotnet publish "FAIR.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Runtime URL binding can be overridden from docker-compose/.env
ENV ASPNETCORE_URLS=http://+:80;https://+:443
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FAIR.API.dll"]
