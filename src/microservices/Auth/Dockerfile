FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/microservices/Auth/Auth.csproj", "microservices/Auth/"]
COPY ["src/microservices/Shared/Common/Common.csproj", "microservices/Shared/Common/"]
COPY ["src/microservices/Shared/Infrastructure/Infrastructure.csproj", "microservices/Shared/Infrastructure/"]
RUN dotnet restore "microservices/Auth/Auth.csproj"
COPY . .
WORKDIR "/src/microservices/Auth"
RUN dotnet build "Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.dll"]