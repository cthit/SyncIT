FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["Sync/Sync.csproj", "Sync/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Bitwarden CLI for automated user confirmation
RUN apt-get update && apt-get install -y --no-install-recommends curl unzip && \
    curl -fsSL "https://github.com/bitwarden/clients/releases/download/cli-v2026.4.1/bw-linux-2026.4.1.zip" -o /tmp/bw.zip && \
    unzip /tmp/bw.zip -d /usr/local/bin && \
    chmod +x /usr/local/bin/bw && \
    rm /tmp/bw.zip && \
    apt-get purge -y --auto-remove curl unzip && \
    rm -rf /var/lib/apt/lists/*

RUN mkdir -p /data
ENV DATABASE_PATH=/data/syncit.db
ENTRYPOINT ["dotnet", "Web.dll"]
