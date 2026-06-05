FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FrontierWeb/FrontierWeb.Api.csproj", "FrontierWeb/"]
COPY ["FrontierWeb.Application/FrontierWeb.Application.csproj", "FrontierWeb.Application/"]
COPY ["FrontierWeb.Domain/FrontierWeb.Domain.csproj", "FrontierWeb.Domain/"]
COPY ["FrontierWeb.Infrastructure/FrontierWeb.Infrastructure.csproj", "FrontierWeb.Infrastructure/"]

RUN dotnet restore "FrontierWeb/FrontierWeb.Api.csproj"

COPY . .

RUN dotnet publish "FrontierWeb/FrontierWeb.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FrontierWeb.Api.dll"]
