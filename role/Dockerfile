FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["role/role_api.csproj", "role/"]
RUN dotnet restore "role/role_api.csproj"
COPY . .
WORKDIR "/src/role"
RUN dotnet build "role_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "role_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "role_api.dll"]
