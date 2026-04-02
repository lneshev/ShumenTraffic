# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["ShumenTraffic.Web.WebAPI/ShumenTraffic.Web.WebAPI.csproj", "ShumenTraffic.Web.WebAPI/"]
COPY ["ShumenTraffic.Common.Services/ShumenTraffic.Common.Services.csproj", "ShumenTraffic.Common.Services/"]
COPY ["ShumenTraffic.Common.Core/ShumenTraffic.Common.Core.csproj", "ShumenTraffic.Common.Core/"]
COPY ["ShumenTraffic.Common.DataAccess/ShumenTraffic.Common.DataAccess.csproj", "ShumenTraffic.Common.DataAccess/"]
COPY ["ShumenTraffic.Web.Services/ShumenTraffic.Web.Services.csproj", "ShumenTraffic.Web.Services/"]
COPY ["ShumenTraffic.Web.Core/ShumenTraffic.Web.Core.csproj", "ShumenTraffic.Web.Core/"]
RUN dotnet restore "./ShumenTraffic.Web.WebAPI/ShumenTraffic.Web.WebAPI.csproj"
COPY . .
WORKDIR "/src/ShumenTraffic.Web.WebAPI"
RUN dotnet build "./ShumenTraffic.Web.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShumenTraffic.Web.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShumenTraffic.Web.WebAPI.dll"]