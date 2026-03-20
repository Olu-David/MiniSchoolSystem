# ── Stage 1: Build (Using 9.0) ──────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy specifically the .csproj to avoid the NETSDK1194 warning
COPY ["MiniSchoolSystem.csproj", "./"]
RUN dotnet restore "MiniSchoolSystem.csproj"

# Copy everything else and publish
COPY . .
# We target the .csproj directly here too
RUN dotnet publish "MiniSchoolSystem.csproj" -c Release -o /app/out /p:UseAppHost=false

# ── Stage 2: Runtime (Using 9.0) ─────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published output
COPY --from=build /app/out ./

# RENDER FIX: 
# Render automatically injects a PORT variable. 
# We tell ASP.NET to listen on THAT port specifically.
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]