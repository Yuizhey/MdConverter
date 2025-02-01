FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MdConverter.Api/MdConverter.Api.csproj", "MdConverter.Api/"]
COPY ["MdConverter.Application/MdConverter.Application.csproj", "MdConverter.Application/"]
COPY ["MdConverter.Core/MdConverter.Core.csproj", "MdConverter.Core/"]
COPY ["MdConverter.DataAccess/MdConverter.DataAccess.csproj", "MdConverter.DataAccess/"]
COPY ["MdConverter.FileStorage/MdConverter.FileStorage.csproj", "MdConverter.FileStorage/"]
RUN dotnet restore "MdConverter.Api/MdConverter.Api.csproj"
COPY . .
WORKDIR "/src/MdConverter.Api"
RUN dotnet build "MdConverter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MdConverter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MdConverter.Api.dll"]
