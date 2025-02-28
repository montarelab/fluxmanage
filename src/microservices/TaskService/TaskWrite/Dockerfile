FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/microservices/TaskService/TaskWrite/TaskWrite.csproj", "src/microservices/TaskService/TaskWrite/"]
COPY ["src/microservices/Shared/Common/Common.csproj", "src/microservices/Shared/Common/"]
COPY ["src/microservices/TaskService/TaskCommon/TaskCommon.csproj", "src/microservices/TaskService/TaskCommon/"]
COPY ["src/microservices/Shared/Infrastructure/Infrastructure.csproj", "src/microservices/Shared/Infrastructure/"]
RUN dotnet restore "src/microservices/TaskService/TaskWrite/TaskWrite.csproj"
COPY . .
WORKDIR "/src/src/microservices/TaskService/TaskWrite"
RUN dotnet build "TaskWrite.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaskWrite.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskWrite.dll"]
