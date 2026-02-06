# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["StreetBites.csproj", "."]
RUN dotnet restore "StreetBites.csproj"

COPY . .
RUN dotnet build "StreetBites.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StreetBites.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "StreetBites.dll"]
