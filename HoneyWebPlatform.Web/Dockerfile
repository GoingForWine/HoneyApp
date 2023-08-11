#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HoneyWebPlatform.Web/HoneyWebPlatform.Web.csproj", "HoneyWebPlatform.Web/"]
COPY ["HoneyWebPlatform.Common/HoneyWebPlatform.Common.csproj", "HoneyWebPlatform.Common/"]
COPY ["HoneyWebPlatform.Data.Models/HoneyWebPlatform.Data.Models.csproj", "HoneyWebPlatform.Data.Models/"]
COPY ["HoneyWebPlatform.Data/HoneyWebPlatform.Data.csproj", "HoneyWebPlatform.Data/"]
COPY ["HoneyWebPlatform.Services.Data.Models/HoneyWebPlatform.Services.Data.Models.csproj", "HoneyWebPlatform.Services.Data.Models/"]
COPY ["HoneyWebPlatform.Web.ViewModels/HoneyWebPlatform.Web.ViewModels.csproj", "HoneyWebPlatform.Web.ViewModels/"]
COPY ["HoneyWebPlatform.Services.Mapping/HoneyWebPlatform.Services.Mapping.csproj", "HoneyWebPlatform.Services.Mapping/"]
COPY ["HoneyWebPlatform.Services.Data/HoneyWebPlatform.Services.Data.csproj", "HoneyWebPlatform.Services.Data/"]
COPY ["HoneyWebPlatform.Web.Infrastructure/HoneyWebPlatform.Web.Infrastructure.csproj", "HoneyWebPlatform.Web.Infrastructure/"]
RUN dotnet restore "HoneyWebPlatform.Web/HoneyWebPlatform.Web.csproj"
COPY . .
WORKDIR "/src/HoneyWebPlatform.Web"
RUN dotnet build "HoneyWebPlatform.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HoneyWebPlatform.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HoneyWebPlatform.Web.dll"]