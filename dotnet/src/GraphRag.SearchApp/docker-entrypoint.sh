#!/bin/sh
# Docker entrypoint for GraphRAG Search App
# Seeds demo data if no listing.json exists, then starts the app.

DATA_ROOT="${SearchApp__DataRoot:-/data/output}"
LISTING="$DATA_ROOT/listing.json"

if [ ! -f "$LISTING" ]; then
    echo "No dataset found at $LISTING — seeding demo data..."
    dotnet GraphRag.SearchApp.dll --seed --run "$@"
else
    echo "Dataset found at $LISTING — starting app..."
    dotnet GraphRag.SearchApp.dll "$@"
fi
