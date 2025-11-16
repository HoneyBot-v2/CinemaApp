# Cinema Application

This solution contains:
- `src/Cinema.API` – ASP.NET Core Web API for the cinema backend (movies, screenings, reservations, users).
- `src/Cinema.MAUI` – .NET MAUI client application.

This README focuses on the Docker setup for the API.

## Dockerfile Overview (`src/Cinema.API/Dockerfile`)

The API Dockerfile is a **multi-stage Docker build** consisting of:

1. **Build stage**
   - Base image: `mcr.microsoft.com/dotnet/sdk:10.0`.
   - Restores NuGet packages for `Cinema.API.csproj`.
   - Copies the full source tree and runs:
     ```bash
     dotnet publish src/Cinema.API/Cinema.API.csproj -c Release -o /app/publish /p:UseAppHost=false
     ```
   - Output is a self-contained, published build in `/app/publish`.

2. **Runtime stage**
   - Base image: `mcr.microsoft.com/dotnet/aspnet:10.0`.
   - Sets the working directory to `/app`.
   - Configures ASP.NET Core to listen on port `8080` inside the container:
     ```bash
     ASPNETCORE_URLS=http://+:8080
     ```
   - Exposes port `8080`.
   - Copies the published output from the build stage and starts the API with:
     ```bash
     dotnet Cinema.API.dll
     ```

The multi-stage approach keeps the final image smaller by excluding the SDK and build-time artifacts from the runtime image.

## Building the Docker Image

From the solution root (`Cinema Application`), build the API image with:

```powershell
cd "c:\Projects\Udemy\Build Real World Cinema Ticket Booking App with .NET MAUI\Cinema Application"

docker build -f src/Cinema.API/Dockerfile -t cinema-api .
```

- `-f src/Cinema.API/Dockerfile` tells Docker where the Dockerfile is.
- `-t cinema-api` names (tags) the image `cinema-api`.
- `.` is the build context (the solution root).

## Running the API Container

Run the API in a container and map it to a host port (e.g. 8080):

```powershell
docker run --rm -p 8080:8080 --name cinema-api cinema-api
```

- `-p 8080:8080` maps host port 8080 to container port 8080.
- `--rm` cleans up the container when it stops.
- `--name cinema-api` gives the container a friendly name.

Once running, you can access the API (for example) at:

- `http://localhost:8080` (root)
- Any Swagger/health endpoints exposed by `Cinema.API` (if configured in `Program.cs`).

## Configuring Environment Variables (DB, JWT, etc.)

The Dockerfile includes commented-out environment variable lines for configuration:

```dockerfile
# ENV ConnectionStrings__ApiDbContextConnection=""
# ENV JWT__Key=""
# ENV JWT__Issuer=""
# ENV JWT__Audience=""
```

Rather than hard-coding these in the image, you typically pass them at **run time**:

```powershell
docker run --rm -p 8080:8080 --name cinema-api `
  -e "ConnectionStrings__ApiDbContextConnection=Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True" `
  -e "JWT__Key=your_jwt_key_here" `
  -e "JWT__Issuer=your_issuer" `
  -e "JWT__Audience=your_audience" `
  cinema-api
```

ASP.NET Core automatically maps these environment variables into the configuration system, matching the nested keys in `appsettings.json` (e.g. `ConnectionStrings:ApiDbContextConnection`).

## Using Docker Compose (Optional)

If you later add a `docker-compose.yml` (for example, to run the API with a database), the `Dockerfile` remains the same. A simple `docker-compose.yml` snippet might look like:

```yaml
services:
  cinema-api:
    build:
      context: .
      dockerfile: src/Cinema.API/Dockerfile
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__ApiDbContextConnection: "Server=db;Database=CinemaDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True"
      JWT__Key: "your_jwt_key_here"
      JWT__Issuer: "your_issuer"
      JWT__Audience: "your_audience"
```

## Purpose of the Docker Setup

- **Consistency**: Ensures the API runs in the same environment across development, testing, and production.
- **Portability**: Allows deploying the cinema backend to any Docker-enabled host or cloud service.
- **Isolation**: Keeps API dependencies isolated from the host machine.
- **Scalability**: Makes it easy to scale out multiple instances of `Cinema.API` behind a load balancer or orchestrator (Kubernetes, Docker Swarm, etc.).

At this time, the Dockerfile only covers the backend API. The .NET MAUI client is typically run on your local machine/emulator rather than inside Docker.
