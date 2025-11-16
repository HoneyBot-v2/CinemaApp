# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/Cinema.API/Cinema.API.csproj", "src/Cinema.API/"]
RUN dotnet restore "src/Cinema.API/Cinema.API.csproj"

# Copy the rest of the source
COPY . .

# Publish
RUN dotnet publish "src/Cinema.API/Cinema.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Listen on HTTP (8080) in container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Optional: set environment for DB/JWT via docker -e or compose
# ENV ConnectionStrings__ApiDbContextConnection=""
# ENV JWT__Key=""
# ENV JWT__Issuer=""
# ENV JWT__Audience=""

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cinema.API.dll"]