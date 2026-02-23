# Copyright (c) Microsoft Corporation. All rights reserved.
# Dockerfile for GraphRAG Blazor Search App
# Multi-stage build: restore → build → publish → runtime

# --- Stage 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution-level files first for layer caching
COPY Directory.Build.props Directory.Packages.props nuget.config global.json GraphRag.slnx ./

# Copy project files for restore
COPY src/GraphRag.Common/GraphRag.Common.csproj src/GraphRag.Common/
COPY src/GraphRag.Storage/GraphRag.Storage.csproj src/GraphRag.Storage/
COPY src/GraphRag.Cache/GraphRag.Cache.csproj src/GraphRag.Cache/
COPY src/GraphRag.Chunking/GraphRag.Chunking.csproj src/GraphRag.Chunking/
COPY src/GraphRag.Input/GraphRag.Input.csproj src/GraphRag.Input/
COPY src/GraphRag.Llm/GraphRag.Llm.csproj src/GraphRag.Llm/
COPY src/GraphRag.Vectors/GraphRag.Vectors.csproj src/GraphRag.Vectors/
COPY src/GraphRag/GraphRag.csproj src/GraphRag/
COPY src/GraphRag.SearchApp/GraphRag.SearchApp.csproj src/GraphRag.SearchApp/

# Restore (cached unless project files change)
RUN dotnet restore src/GraphRag.SearchApp/GraphRag.SearchApp.csproj

# Copy all source code
COPY src/ src/

# Build and publish
RUN dotnet publish src/GraphRag.SearchApp/GraphRag.SearchApp.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# --- Stage 2: Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser -d /app appuser

# Copy published output
COPY --from=build /app/publish .

# Default output directory for dataset files
RUN mkdir -p /data/output && chown -R appuser:appuser /data

# Switch to non-root user
USER appuser

# Expose default port
EXPOSE 8080

# Configure ASP.NET Core to listen on 8080 (matches Python app convention)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV SearchApp__DataRoot=/data/output

ENTRYPOINT ["dotnet", "GraphRag.SearchApp.dll"]
