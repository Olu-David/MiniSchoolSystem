FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy EVERYTHING first to make sure we don't miss the project file
COPY . .

# Find any .csproj file and restore it
RUN dotnet restore

# Build the project
RUN dotnet publish -c Release -o out

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# IMPORTANT: Make sure this name matches your ACTUAL .dll file
ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]
