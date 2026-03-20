# 1. Use the full SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# FIX: Force the system to find the SDK
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:$DOTNET_ROOT

# 2. Standard copy/restore
COPY ["MiniSchoolSystem.csproj", "./"]
RUN dotnet restore "MiniSchoolSystem.csproj"

# 3. Copy all and publish
COPY . .
# We target the .csproj to fix the 'NETSDK1194' warning we saw earlier
RUN dotnet publish "MiniSchoolSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# PORT FIX: Render looks for port 10000 by default
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]