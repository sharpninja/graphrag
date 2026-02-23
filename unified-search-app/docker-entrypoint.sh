#!/bin/sh
# Docker entrypoint for GraphRAG Unified Search App (Python/Streamlit)
# Seeds demo data if no listing.json exists, then starts the app.

DATA_ROOT="${DATA_ROOT:-/data}"
LISTING="$DATA_ROOT/listing.json"

if [ ! -f "$LISTING" ]; then
    echo "No dataset found at $LISTING — seeding demo data..."
    /.venv/bin/python seed_data.py --data-root "$DATA_ROOT"
else
    echo "Dataset found at $LISTING — starting app..."
fi

exec /.venv/bin/streamlit run app/home_page.py --server.port=8501 --server.address=0.0.0.0
