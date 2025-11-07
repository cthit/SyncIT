# SyncIT

SyncIT is web app to keep the IT student division's account system [Gamma](https://github.com/cthit/Gamma) synchronized
with external services,
primarily GSuite and Bitwarden. It allows the user (often the IT-manager in the division board) to select what changes
to apply from a web-based UI.

## Configuration

The account sources and targets are configured via the web ui and are then stored in the database.
The application itself requires some environment variables to be set to know where to store the SQLite database
and what OpenID Connect provider to use for authentication (designed for Gamma but any should work).

The OIDC provider is the only guard against unauthorized access to the web UI, so it is important to set this up
correctly.
If the OIDC provider allows a user through, they will be able to run syncs and see potentially sensitive user data.
The OAuth redirect URI should be set to `https://<your-domain>/signin-oidc`.

The following environment variables are used and required by the application:

- `DATABASE_PATH`: Path to the SQLite database. Example and default in Docker: `/data/syncit.db`. If not set, defaults
  to `syncit.db` in the working directory.
- `OIDC__Authority`: URL to the OpenID Connect provider. Example for Gamma: `https://auth.chalmers.it`
- `OIDC__ClientId`: Client ID registered with the OIDC provider.
- `OIDC__ClientSecret`: Client secret registered with the OIDC provider.

If you're running SyncIT behind a reverse proxy, the application supports configuring trusted networks and proxy IPs
used for processing X-Forwarded-* headers.
This is important for OIDC to correctly determine the original request scheme and host.

These settings are configured as comma-delimited strings which makes them easy to set via environment variables:

- `ForwardedHeaders__KnownNetworks`: Optional. Comma-delimited list of CIDR networks that should be treated as
  trusted for forwarded headers. Example: `10.0.0.0/8,192.168.0.0/16`.
- `ForwardedHeaders__KnownProxies`: Optional. Comma-delimited list of proxy IP addresses (IPv4/IPv6). Example:
  `203.0.113.1,198.51.100.4`.

These settings can also be set in an `appsettings.json` file, but environment variables take precedence.
If these are not set the application will not trust any proxies for forwarded headers.

The easiest and recommended way to run SyncIT is via Docker. See the `docker-compose.yml` file for an example of how to
set the environment variables.
A pre-built image is available on GitHub: `ghcr.io/cthit/syncit:latest`.

## Development setup

### Prerequisites

- .NET SDK 9.0 or later (the projects target net9.0)
- Optional: Docker & docker-compose for containerized runs
- Optional: `dotnet-ef` global tool for running EF Core migrations locally

### Quick start — development (local)

1. From repository root, build the solution:

```bash
dotnet build
```

2. Run the Web project (Blazor UI). From repo root:

```bash
cd Web
dotnet run --configuration Debug
```

3. Open the application in your browser (`https://localhost:7189`).

4. Configure account sources and targets via the Web UI under 'Settings' -> 'Account Services'. The JSON provider can be
   used for local testing
   and development.

5. Use the Web UI to run syncs by selecting what changes to sync and then press apply. The Web app uses a SQLite file (
   `syncit.db`) by default in
   development.

Running with Docker (recommended for simple deployment)

Build and run the containers with Docker Compose:

```bash
docker compose up --build
```

This will build the images and start the Web UI. Configure environment variables (see "Configuration" below) before
starting if you want external services to be reachable.

### Database migrations

This project uses EF Core migrations stored under `Web/Database/Migrations`.
These are automatically applied on application startup.

To generate new migrations locally, use the `dotnet-ef` tool. From the `Web/` directory, run:

```bash
dotnet ef migrations add <MigrationName> --context SyncItDbContext --output-dir Database/Migrations
```