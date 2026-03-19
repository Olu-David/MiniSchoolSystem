FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MiniSchoolSystem.csproj", "."]
RUN dotnet restore "./MiniSchoolSystem.csproj"
COPY . .
RUN dotnet publish "MiniSchoolSystem.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MiniSchoolSystem.dll"]