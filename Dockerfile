FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/StockMapSvelte.Api/StockMapSvelte.Api.csproj", "StockMapSvelte.Api/"]
COPY ["src/StockMapSvelte.Application/StockMapSvelte.Application.csproj", "StockMapSvelte.Application/"]
COPY ["src/StockMapSvelte.Domain/StockMapSvelte.Domain.csproj", "StockMapSvelte.Domain/"]
COPY ["src/StockMapSvelte.Infrastructure/StockMapSvelte.Infrastructure.csproj", "StockMapSvelte.Infrastructure/"]

RUN dotnet restore "StockMapSvelte.Api/StockMapSvelte.Api.csproj"

COPY src/ .

WORKDIR "/src/StockMapSvelte.Api"
RUN dotnet build "StockMapSvelte.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockMapSvelte.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# EXPOSE 8080 
# ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet StockMapSvelte.Api.dll

# ENTRYPOINT ["dotnet", "StockMapSvelte.Api.dll"]