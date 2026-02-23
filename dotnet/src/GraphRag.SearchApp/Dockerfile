# Copyright (c) Microsoft Corporation. All rights reserved.
# Dockerfile for GraphRAG Blazor Search App
# Requires pre-built publish output:
#   dotnet publish src/GraphRag.SearchApp/GraphRag.SearchApp.csproj \
#     -c Release -o dotnet/publish --self-contained -r linux-x64 /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app

# Copy pre-built single-file publish output
COPY publish/ .

# Copy entrypoint script (fix Windows CRLF → LF)
COPY src/GraphRag.SearchApp/docker-entrypoint.sh /app/docker-entrypoint.sh
RUN sed -i 's/\r$//' /app/docker-entrypoint.sh && chmod +x /app/docker-entrypoint.sh \
    && chmod +x /app/GraphRag.SearchApp \
    && mkdir -p /data/output

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV SearchApp__DataRoot=/data/output

ENTRYPOINT ["/app/docker-entrypoint.sh"]
