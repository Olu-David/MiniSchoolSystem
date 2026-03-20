# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the .csproj first to cache your NuGet packages (saves time!)
COPY ["MiniSchoolSystem.csproj", "./"]
RUN dotnet restore "./MiniSchoolSystem.csproj"

# Copy everything else
COPY . .

# IMPORTANT: Run publish specifically on the .csproj to fix that 'NETSDK1194' warning
# and use /p:UseAppHost=false to keep it clean for Linux
RUN dotnet publish "MiniSchoolSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Final Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Make sure this matches your project's output DLL name exactly
ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]