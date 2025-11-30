# Stage 1: build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY src/Cinema.API/Cinema.API.csproj ./Cinema.API/
RUN dotnet restore ./Cinema.API/Cinema.API.csproj

# Copy the rest of the source and publish
COPY src/Cinema.API ./Cinema.API
WORKDIR /src/Cinema.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: runtime image (small)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Where the SQLite DB file will live
RUN mkdir /data

# Configure Kestrel to listen on 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy published output from build stage
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Cinema.API.dll"]
