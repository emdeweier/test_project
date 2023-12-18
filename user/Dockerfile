FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["user/user_api.csproj", "ocsp_user/"]
RUN dotnet restore "user/user_api.csproj"
COPY . .
WORKDIR "/src/user"
RUN dotnet build "user_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "user_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "user_api.dll"]
