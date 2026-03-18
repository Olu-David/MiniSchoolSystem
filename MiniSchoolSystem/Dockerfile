# 1. THE BUILD STAGE (The "Kitchen Prep")
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies (NuGet packages)
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# 2. THE RUNTIME STAGE (The "Serving" Stage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the compiled files from the build stage
COPY --from=build /app/out .

# THE START COMMAND (This is the "dotnet MiniSchoolSystem.dll" part)
ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]