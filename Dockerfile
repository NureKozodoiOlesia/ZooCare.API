# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ZooCare.API/ZooCare.API.csproj", "ZooCare.API/"]
RUN dotnet restore "ZooCare.API/ZooCare.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ZooCare.API"
RUN dotnet build "ZooCare.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ZooCare.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZooCare.API.dll"]

