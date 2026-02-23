# Getting Started with the GraphRAG Search App

A step-by-step guide to setting up, configuring, and running the **GraphRAG Blazor Search App** — an interactive web UI for searching and exploring GraphRAG-indexed knowledge graphs.

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Quick Start with Demo Data](#quick-start-with-demo-data)
4. [Build & Run](#build--run)
5. [Docker](#docker)
6. [Preparing Your Data](#preparing-your-data)
7. [Seed Data Reference](#seed-data-reference)
8. [Configuration Reference](#configuration-reference)
9. [Using the App](#using-the-app)
10. [Data Sources](#data-sources)
11. [Architecture](#architecture)
12. [Extending the App](#extending-the-app)
13. [Troubleshooting](#troubleshooting)

---

## Overview

The **GraphRag.SearchApp** is a .NET Blazor Server application that provides a browser-based interface for querying GraphRAG-indexed datasets. It is the .NET equivalent of the Python/Streamlit `unified-search-app`.

### What It Does

- Run **Global**, **Local**, **DRIFT**, and **Basic RAG** searches in parallel against your indexed data
- View search results with Markdown-rendered LLM responses and performance statistics (completion time, LLM calls, token usage)
- Browse and filter **Community Reports** by community level
- Switch between **multiple datasets** at runtime
- Connect to **local files** (Parquet/CSV) or **Azure Blob Storage**

### Technology Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | .NET 10 / Blazor Server |
| **UI Library** | [MudBlazor](https://mudblazor.com/) 8.5.0 |
| **Markdown** | [Markdig](https://github.com/xoofx/markdig) 0.38.0 |
| **Data Format** | [Parquet.Net](https://github.com/aloneguid/parquet-dotnet) 5.3.0 |
| **Cloud Storage** | Azure.Storage.Blobs + Azure.Identity |
| **Pattern** | MVVM with `ObservableCollection<T>` |

---

## Prerequisites

| Requirement | Version | Notes |
|------------|---------|-------|
| **.NET SDK** | 10.0.100 or later | [Download .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **GraphRAG indexed data** | Any version | Output from a GraphRAG indexing run (Python or .NET) |

Verify your .NET installation:

```bash
dotnet --version
# Should show 10.0.x
```

> **Note:** You do _not_ need Azure OpenAI credentials to browse already-indexed data in the Community Explorer. You _do_ need a configured LLM to run live searches.

---

## Quick Start with Demo Data

The fastest way to explore the SearchApp is with the built-in **"A Christmas Carol"** demo dataset. No GraphRAG indexing run or LLM keys are needed — the app ships with a seed data generator that creates Parquet files you can browse immediately in the Community Explorer.

### Option A: Docker (Recommended)

```bash
cd dotnet
docker build -f src/GraphRag.SearchApp/Dockerfile -t graphrag-search-app .
docker run -d --name graphrag-search -p 8080:8080 graphrag-search-app
```

Open `http://localhost:8080`. The container automatically seeds demo data on first startup if no existing dataset is found. You'll see **"A Christmas Carol"** in the dataset dropdown.

### Option B: Local .NET

```bash
cd dotnet/src/GraphRag.SearchApp

# Seed the demo data into ./output
dotnet run -- --seed

# Now run the app (it reads from ./output)
dotnet run
```

Or seed and run in one step:

```bash
dotnet run -- --seed --run
```

Open `https://localhost:5001` (or `http://localhost:5000`).

### What You Get

The demo dataset contains data from Charles Dickens' *A Christmas Carol*, pre-indexed into the GraphRAG format:

| Table | Count | Examples |
|-------|-------|----------|
| **Entities** | 8 | Ebenezer Scrooge, Bob Cratchit, Tiny Tim, Jacob Marley, Ghost of Christmas Past/Present/Yet to Come, Fred |
| **Relationships** | 10 | Scrooge → Cratchit (employer), Marley → Scrooge (warns), Cratchit → Tiny Tim (father), Ghost of Christmas Present → Scrooge (guides) |
| **Communities** | 2 | "Scrooge's Transformation" (5 members), "The Cratchit Family" (3 members) |
| **Community Reports** | 2 | Scrooge's Redemption Arc (rank 9.5), The Cratchit Family Hardship (rank 8.0) |
| **Text Units** | 5 | Source passages including *"Marley was dead: to begin with…"*, *"Bah! Humbug!"*, *"God bless us every one!"* |

### Try It Out

Once the app is running with demo data:

1. **Community Explorer tab** — Select "A Christmas Carol" from the dataset dropdown, then browse the two community reports:
   - *Scrooge's Redemption Arc* — covers the supernatural intervention, journey through time, and Scrooge's transformation
   - *The Cratchit Family Hardship* — covers the family's resilience, Tiny Tim's illness, and Bob's loyalty

2. **Search tab** — Try sample queries against the demo data (requires LLM configuration):
   - `"What is the relationship between Scrooge and Tiny Tim?"`
   - `"How does Jacob Marley influence Scrooge's transformation?"`
   - `"What role do the three ghosts play in the story?"`
   - `"Describe the Cratchit family's Christmas celebration"`

> **Note:** Live search queries require an LLM endpoint. Without one, you can still explore Community Reports and browse entities/relationships in the dataset.

---

## Build & Run

### Clone and Build

```bash
git clone https://github.com/microsoft/graphrag.git
cd graphrag/dotnet

# Restore and build the entire solution (includes SearchApp)
dotnet build
```

### Run the App

```bash
cd src/GraphRag.SearchApp
dotnet run
```

The app starts and listens on:

| Protocol | URL |
|----------|-----|
| HTTPS | `https://localhost:5001` |
| HTTP | `http://localhost:5000` |

Open `https://localhost:5001` in your browser.

### Run in Development Mode

```bash
dotnet run --environment Development
```

Development mode enables:
- Detailed error pages
- Verbose logging (all levels set to `Information`)
- Hot reload support with `dotnet watch`

```bash
# Hot reload — auto-rebuilds when files change
dotnet watch run
```

---

## Docker

The Search App includes Docker support for containerized deployment, mirroring the Python `unified-search-app` Dockerfile pattern.

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) 20.10 or later
- [Docker Compose](https://docs.docker.com/compose/install/) v2 (included with Docker Desktop)

### Quick Start with Docker Compose

The simplest way to run the app in Docker:

```bash
cd dotnet/src/GraphRag.SearchApp

# Build and start — demo data is seeded automatically on first run
docker compose up --build
```

The app is available at `http://localhost:8080`. On first startup, the container detects that no `listing.json` exists and automatically seeds the **"A Christmas Carol"** demo dataset. Subsequent restarts skip seeding.

To bring your own data instead, place your indexed output in `./output/` before starting:

```bash
# Place your indexed data (overrides auto-seeding)
cp -r /path/to/your/graphrag-output/* ./output/
docker compose up --build
```

### Build the Image Directly

```bash
# From the dotnet/ directory (build context needs solution-level files)
cd dotnet
docker build -f src/GraphRag.SearchApp/Dockerfile -t graphrag-search-app .
```

### Run the Container

```bash
# Basic run with local data mount
docker run -d \
  --name graphrag-search \
  -p 8080:8080 \
  -v /path/to/your/output:/data/output:ro \
  graphrag-search-app

# With Azure Blob Storage
docker run -d \
  --name graphrag-search \
  -p 8080:8080 \
  -e SearchApp__BlobAccountName=mystorageaccount \
  -e SearchApp__BlobContainerName=graphrag-data \
  -e AZURE_CLIENT_ID=your-client-id \
  -e AZURE_TENANT_ID=your-tenant-id \
  -e AZURE_CLIENT_SECRET=your-client-secret \
  graphrag-search-app
```

### Docker Compose Configuration

The `docker-compose.yml` file is in `dotnet/src/GraphRag.SearchApp/`:

```yaml
services:
  search-app:
    build:
      context: ../../
      dockerfile: src/GraphRag.SearchApp/Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - SearchApp__DataRoot=/data/output
    volumes:
      - ./output:/data/output:ro
    restart: unless-stopped
```

Customize by uncommenting the Azure Blob environment variables or changing the volume mount path.

### Comparison with Python Docker Setup

| | Python (`unified-search-app`) | .NET (`GraphRag.SearchApp`) |
|---|---|---|
| **Base image** | `mcr.microsoft.com/oryx/python:3.11` | `mcr.microsoft.com/dotnet/aspnet:10.0-preview` |
| **Build** | Single-stage (uv sync) | Multi-stage (restore → publish → runtime) |
| **Port** | 8501 (Streamlit) | 8080 (Kestrel) |
| **Entry point** | `uv run poe start_prod` | `docker-entrypoint.sh` (auto-seeds, then runs app) |
| **Data mount** | N/A (env vars) | `/data/output` volume |
| **Non-root user** | No | Yes (`appuser`) |
| **Demo data** | No built-in seed | Auto-seeds "A Christmas Carol" on first run |

---

## Preparing Your Data

The Search App reads data produced by a GraphRAG indexing run. You need:

1. **Indexed output files** — Parquet files generated by `graphrag index`
2. **A listing file** — JSON file describing available datasets

> **Tip:** If you just want to explore the app, use the built-in seed data instead (see [Quick Start with Demo Data](#quick-start-with-demo-data)). The instructions below are for loading your own indexed data.

### Expected Output Files

After running `graphrag index`, your output directory should contain:

```
output/
├── entities.parquet             # Extracted entities (name, type, description, rank)
├── relationships.parquet        # Entity relationships (source, target, weight, description)
├── communities.parquet          # Detected communities (title, level, size)
├── community_reports.parquet    # Generated community reports (summary, full_content, rank)
├── text_units.parquet           # Source text chunks (text, n_tokens, document_id)
└── covariates.parquet           # (optional) Extracted covariates
```

For reference, the built-in seed data creates files with the following schemas:

**entities.parquet** columns:
| Column | Type | Example |
|--------|------|---------|
| `id` | string | `"e-0"` |
| `short_id` | string | `"0"` |
| `title` | string | `"EBENEZER SCROOGE"` |
| `type` | string | `"PERSON"` |
| `description` | string | `"Ebenezer Scrooge is a miserly London businessman…"` |
| `rank` | int | `10` |

**relationships.parquet** columns:
| Column | Type | Example |
|--------|------|---------|
| `id` | string | `"r-0"` |
| `short_id` | string | `"0"` |
| `source` | string | `"EBENEZER SCROOGE"` |
| `target` | string | `"BOB CRATCHIT"` |
| `weight` | double | `8.0` |
| `description` | string | `"Scrooge employs Cratchit as his clerk…"` |

**community_reports.parquet** columns:
| Column | Type | Example |
|--------|------|---------|
| `id` | string | `"cr-0"` |
| `short_id` | string | `"0"` |
| `title` | string | `"Scrooge's Redemption Arc"` |
| `community_id` | string | `"0"` |
| `summary` | string | `"This community centers on Ebenezer Scrooge…"` |
| `full_content` | string | Full Markdown report |
| `rank` | double | `9.5` |
| `size` | int | `5` |

### Create a Dataset Listing File

Create a `listing.json` file in your data root directory. Here is the listing file the seed data creates:

```json
[
  {
    "key": "christmas-carol",
    "path": "christmas-carol",
    "name": "A Christmas Carol",
    "description": "GraphRAG index of Charles Dickens' A Christmas Carol — demo dataset for the SearchApp.",
    "communityLevel": 1
  }
]
```

For multiple datasets, add additional entries:

```json
[
  {
    "key": "christmas-carol",
    "path": "christmas-carol",
    "name": "A Christmas Carol",
    "description": "GraphRAG index of Charles Dickens' A Christmas Carol — demo dataset for the SearchApp.",
    "communityLevel": 1
  },
  {
    "key": "my-corpus",
    "path": "my-corpus/output",
    "name": "My Research Corpus",
    "description": "Knowledge graph built from research papers",
    "communityLevel": 0
  }
]
```

Each entry has:

| Field | Type | Description |
|-------|------|-------------|
| `key` | `string` | Unique identifier for the dataset |
| `path` | `string` | Relative path from `DataRoot` to the output directory, or absolute path |
| `name` | `string` | Display name shown in the sidebar |
| `description` | `string` | Brief description of the dataset |
| `communityLevel` | `int` | Default community level for filtering reports (0 = top level) |

### Example Directory Layout

This is the exact layout produced by the seed data generator:

```
./output/                          ← DataRoot
├── listing.json                   ← Dataset listing (256 bytes)
└── christmas-carol/
    └── output/
        ├── entities.parquet       ← 8 entities (3.5 KB)
        ├── relationships.parquet  ← 10 relationships (2.6 KB)
        ├── communities.parquet    ← 2 communities (1.1 KB)
        ├── community_reports.parquet ← 2 reports (13.9 KB)
        └── text_units.parquet     ← 5 text units (4.0 KB)
```

For your own datasets, replicate this structure:

```
./output/                          ← DataRoot
├── listing.json                   ← Dataset listing
├── christmas-carol/               ← Seed data (auto-generated)
│   └── output/
│       ├── entities.parquet
│       └── ...
└── my-corpus/                     ← Your own indexed data
    └── output/
        ├── entities.parquet
        ├── relationships.parquet
        ├── communities.parquet
        ├── community_reports.parquet
        └── text_units.parquet
```

---

## Seed Data Reference

The built-in seed data generator (`SeedDataService`) creates a complete demo dataset that exercises all features of the SearchApp without requiring an external GraphRAG indexing run.

### CLI Arguments

| Argument | Description |
|----------|-------------|
| `--seed` | Generate demo data in `DataRoot`. Exits after seeding unless `--run` is also specified. |
| `--force` | Overwrite existing seed data (use with `--seed`). |
| `--run` | Continue running the app after seeding (use with `--seed`). |

**Examples:**

```bash
# Seed only (creates files and exits)
dotnet run -- --seed

# Seed and start the app
dotnet run -- --seed --run

# Force re-seed (overwrites existing demo data) and start
dotnet run -- --seed --force --run

# Seed to a custom location
SearchApp__DataRoot=/tmp/demo dotnet run -- --seed
```

### Docker Auto-Seed Behavior

The Docker image uses `docker-entrypoint.sh` which implements automatic seeding:

1. On container start, checks if `$DATA_ROOT/listing.json` exists
2. If **missing** → runs `dotnet GraphRag.SearchApp.dll --seed --run` (seeds, then starts app)
3. If **present** → runs `dotnet GraphRag.SearchApp.dll` (starts app directly)

This means:
- **First run** with an empty `/data/output` volume → demo data is auto-created
- **Subsequent runs** → existing data is preserved, no re-seeding
- **With your own data** → mount a volume containing `listing.json` to skip seeding

```bash
# Auto-seed (first run with empty volume)
docker run -d -p 8080:8080 graphrag-search-app

# Skip seeding (provide your own data)
docker run -d -p 8080:8080 -v /my/data:/data/output:ro graphrag-search-app

# Force re-seed inside a running container
docker exec graphrag-search dotnet GraphRag.SearchApp.dll --seed --force
```

### Demo Dataset: A Christmas Carol

The seed data is drawn from Charles Dickens' *A Christmas Carol*, chosen because it's a well-known, public-domain text with clear entity relationships and community structure.

#### Entities (8)

| Title | Type | Rank | Description |
|-------|------|------|-------------|
| **EBENEZER SCROOGE** | PERSON | 10 | Miserly London businessman who undergoes a dramatic moral transformation through supernatural visitation |
| **BOB CRATCHIT** | PERSON | 7 | Scrooge's loyal, underpaid clerk who maintains a loving household despite poverty |
| **TINY TIM** | PERSON | 8 | Cratchit's youngest son, a sickly child whose potential death catalyzes Scrooge's change |
| **JACOB MARLEY** | PERSON | 6 | Scrooge's deceased partner whose ghost warns him of the consequences of greed |
| **GHOST OF CHRISTMAS PAST** | ENTITY | 5 | First spirit, shows scenes from Scrooge's earlier life |
| **GHOST OF CHRISTMAS PRESENT** | ENTITY | 5 | Second spirit, reveals current Christmas celebrations including the Cratchits |
| **GHOST OF CHRISTMAS YET TO COME** | ENTITY | 5 | Third spirit, shows visions of a grim future |
| **FRED** | PERSON | 4 | Scrooge's cheerful nephew who persistently invites him to Christmas dinner |

#### Relationships (10)

| Source → Target | Weight | Description |
|-----------------|--------|-------------|
| Scrooge → Bob Cratchit | 8.0 | Employer/clerk — Scrooge treats Cratchit poorly |
| Scrooge → Jacob Marley | 9.0 | Former business partners — Marley's ghost warns Scrooge |
| Bob Cratchit → Tiny Tim | 10.0 | Father/son — Tim is Cratchit's beloved youngest |
| Jacob Marley → Scrooge | 9.0 | Marley's ghost initiates Scrooge's transformation |
| Ghost of Christmas Past → Scrooge | 7.0 | Takes Scrooge through earlier memories |
| Ghost of Christmas Present → Scrooge | 7.0 | Shows Scrooge how others celebrate the holiday |
| Ghost of Christmas Yet to Come → Scrooge | 8.0 | Reveals a dark future if Scrooge doesn't change |
| Scrooge → Fred | 5.0 | Uncle/nephew — Fred invites Scrooge to dinner |
| Ghost of Christmas Present → Tiny Tim | 6.0 | Shows Scrooge the Cratchit family and Tim's condition |
| Scrooge → Tiny Tim | 7.0 | Scrooge becomes deeply moved by Tim's plight |

#### Community Reports (2)

**"Scrooge's Redemption Arc"** (community 0, rank 9.5, 5 members)
> Centers on Ebenezer Scrooge and the supernatural forces that drive his transformation. Key findings: supernatural intervention via Marley's ghost, journey through time with three spirits, the pivotal moment of Tiny Tim's empty chair, and Scrooge's complete transformation by Christmas morning.

**"The Cratchit Family Hardship"** (community 1, rank 8.0, 3 members)
> Revolves around the Cratchit family's poverty and resilience. Key findings: the family celebrates Christmas with genuine joy despite hardship, Tiny Tim's illness creates narrative urgency, Bob remains loyal to Scrooge despite harsh conditions, and the family's circumstances serve as the primary emotional catalyst for Scrooge's redemption.

#### Text Units (5)

| # | Source Passage | Tokens |
|---|----------------|--------|
| 0 | *"Marley was dead: to begin with. There is no doubt whatever about that…"* | 68 |
| 1 | *"Oh! But he was a tight-fisted hand at the grindstone, Scrooge! a squeezing, wrenching, grasping…"* | 52 |
| 2 | *"'A merry Christmas, uncle! God save you!' cried a cheerful voice… 'Bah!' said Scrooge, 'Humbug!'"* | 53 |
| 3 | *"Bob Cratchit's fire was so very much smaller that it looked like one coal…"* | 47 |
| 4 | *"'God bless us every one!' said Tiny Tim, the last of all…"* | 52 |

---

## Configuration Reference

All configuration is in `appsettings.json` under the `SearchApp` section:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "SearchApp": {
    "DataRoot": "./output",
    "ListingFile": "listing.json",
    "DefaultSuggestedQuestions": 5,
    "CacheTtlSeconds": 604800,
    "BlobAccountName": "",
    "BlobContainerName": ""
  }
}
```

### Configuration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DataRoot` | `string` | `"./output"` | Root directory for local dataset files. Relative paths are resolved from the app's working directory. |
| `ListingFile` | `string` | `"listing.json"` | Name of the JSON file (inside `DataRoot`) that lists available datasets. |
| `DefaultSuggestedQuestions` | `int` | `5` | Number of suggested questions to generate for each dataset. |
| `CacheTtlSeconds` | `int` | `604800` | Cache time-to-live in seconds (default: 7 days). |
| `BlobAccountName` | `string` | `""` | Azure Blob Storage account name. Set this _and_ `BlobContainerName` to use blob storage. |
| `BlobContainerName` | `string` | `""` | Azure Blob Storage container name. |

### Environment-Specific Overrides

Use `appsettings.Development.json` for development overrides:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "SearchApp": {
    "DataRoot": "./test-output"
  }
}
```

You can also use environment variables with the `SearchApp__` prefix:

```bash
# Linux/macOS
export SearchApp__DataRoot=/data/graphrag-output
export SearchApp__BlobAccountName=mystorageaccount

# Windows (PowerShell)
$env:SearchApp__DataRoot = "C:\data\graphrag-output"
$env:SearchApp__BlobAccountName = "mystorageaccount"
```

---

## Using the App

### Search Page (`/` or `/search`)

The Search page is the home page of the application.

1. **Select a dataset** from the sidebar dropdown (e.g., "A Christmas Carol" from the demo data)
2. **Choose search types** using the sidebar toggles:
   - **Global** (on by default) — Community-based global search
   - **Local** (on by default) — Local context search using entity neighborhoods
   - **DRIFT** (off by default) — Dynamic retrieval with iterative feedback
   - **Basic RAG** (off by default) — Traditional vector-similarity search
3. **Type your question** in the search bar and press Enter
4. **View results** — Each enabled search type runs in parallel and displays in its own card with:
   - Markdown-rendered LLM response
   - Performance stats: completion time, LLM calls, token usage
   - Search type icon and label

**Example queries** for the Christmas Carol demo dataset:
- `"What is the relationship between Scrooge and Tiny Tim?"`
- `"How does Jacob Marley influence Scrooge's transformation?"`
- `"What role do the three ghosts play in the story?"`
- `"Describe the Cratchit family's Christmas celebration"`
- `"Why does Scrooge change his ways?"`

**Suggested Questions**: When available, clickable question chips appear above the results. Click any chip to auto-fill and execute that question.

### Community Explorer (`/communities`)

The Community Explorer provides a split-panel view for browsing community reports.

1. **Left panel** — Scrollable list of community reports
   - **Filter by level** using the dropdown at the top
   - Click any report to select it
   - With demo data, you'll see "Scrooge's Redemption Arc" and "The Cratchit Family Hardship"
2. **Right panel** — Full detail view of the selected report showing:
   - Title, community ID, rank, and size
   - Summary text
   - Full Markdown-rendered report content with headers, numbered findings, and bold key terms

### Sidebar

The sidebar is persistent across all pages and provides:

- **Navigation** — Switch between Search and Community Explorer
- **Dataset selector** — Choose which indexed dataset to query
- **Search type toggles** — Enable/disable individual search algorithms
- **Suggested questions count** — Control how many questions are generated (1–20)

---

## Data Sources

The app supports two data source backends, selected automatically based on configuration.

### Local File System (Default)

When `BlobAccountName` is empty, the app reads Parquet files from the local file system.

```json
{
  "SearchApp": {
    "DataRoot": "./output"
  }
}
```

The `DatasetLoader` resolves dataset paths relative to `DataRoot`. Both Parquet (`.parquet`) and CSV (`.csv`) files are supported.

### Azure Blob Storage

When both `BlobAccountName` and `BlobContainerName` are set, the app reads from Azure Blob Storage using `DefaultAzureCredential`.

```json
{
  "SearchApp": {
    "BlobAccountName": "mystorageaccount",
    "BlobContainerName": "graphrag-data"
  }
}
```

**Authentication**: Uses [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential), which tries (in order):

1. Environment variables (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_CLIENT_SECRET`)
2. Azure CLI (`az login`)
3. Visual Studio / VS Code credentials
4. Managed Identity (when deployed to Azure)

**Blob path convention**: The dataset's `path` in `listing.json` becomes the blob prefix. For example, with `"path": "my-dataset/output"`, the app reads `my-dataset/output/entities.parquet`, etc.

---

## Architecture

### Project Structure

```
dotnet/src/GraphRag.SearchApp/
├── App.razor                      # Root component (HTML shell)
├── Routes.razor                   # Router configuration
├── Program.cs                     # Host builder & DI registration
├── _Imports.razor                 # Global Razor using directives
├── Config/
│   ├── SearchAppConfig.cs         # IOptions<T> configuration model
│   └── DatasetConfig.cs           # Dataset descriptor record
├── Models/
│   ├── SearchType.cs              # Enum: Basic, Local, Global, Drift
│   ├── AppSearchResult.cs         # Search result with stats
│   └── KnowledgeModel.cs          # Loaded dataset data container
├── ViewModels/
│   ├── ViewModelBase.cs           # INotifyPropertyChanged base class
│   ├── AppStateViewModel.cs       # Global app state (scoped per circuit)
│   ├── SearchViewModel.cs         # Search page state
│   └── CommunityExplorerViewModel.cs  # Community page state
├── Services/
│   ├── IDatasource.cs             # Data source abstraction
│   ├── LocalFileDatasource.cs     # Local Parquet/CSV reader
│   ├── LocalDatasource.cs         # ITableProvider-based local reader
│   ├── BlobDatasource.cs          # Azure Blob Storage reader
│   ├── DatasetLoader.cs           # Dataset listing & source factory
│   ├── KnowledgeModelService.cs   # Parallel data model loading
│   ├── SearchOrchestrator.cs      # Parallel search execution
│   ├── SeedDataService.cs         # Demo data generator (Christmas Carol)
│   └── MarkdownRenderer.cs        # Markdig pipeline wrapper
├── Layout/
│   ├── MainLayout.razor           # App shell (AppBar + Drawer + Content)
│   ├── MainLayout.razor.cs        # Layout code-behind
│   ├── Sidebar.razor              # Navigation + dataset + search toggles
│   └── Sidebar.razor.cs           # Sidebar code-behind
├── Pages/
│   ├── Search.razor               # Search page UI
│   ├── Search.razor.cs            # Search page logic
│   ├── CommunityExplorer.razor    # Community explorer UI
│   └── CommunityExplorer.razor.cs # Community explorer logic
├── Components/
│   ├── SearchResultPanel.razor    # Individual search result card
│   ├── SearchResultPanel.razor.cs # Result card code-behind
│   ├── QuestionsList.razor        # Suggested questions chips
│   ├── ReportDetails.razor        # Community report detail view
│   └── ReportDetails.razor.cs     # Report detail code-behind
├── Properties/
│   └── launchSettings.json        # Dev server URLs
├── wwwroot/
│   └── css/app.css                # Custom styles
├── appsettings.json               # Production config
├── appsettings.Development.json   # Development overrides
├── docker-entrypoint.sh           # Container startup (auto-seed + run)
├── Dockerfile                     # Multi-stage Docker build
└── docker-compose.yml             # Docker Compose config
```

### Dependency Injection

All services and ViewModels are registered as **Scoped** (one instance per Blazor circuit/connection):

```csharp
// Program.cs
builder.Services.Configure<SearchAppConfig>(
    builder.Configuration.GetSection("SearchApp"));

builder.Services.AddScoped<MarkdownRenderer>();
builder.Services.AddScoped<DatasetLoader>();
builder.Services.AddScoped<KnowledgeModelService>();
builder.Services.AddScoped<SearchOrchestrator>();

builder.Services.AddScoped<AppStateViewModel>();
builder.Services.AddScoped<SearchViewModel>();
builder.Services.AddScoped<CommunityExplorerViewModel>();
```

### MVVM Pattern

ViewModels implement `INotifyPropertyChanged` and expose all collection properties as `ObservableCollection<T>`:

```
ViewModelBase (INotifyPropertyChanged)
├── AppStateViewModel
│   ├── ObservableCollection<DatasetConfig> Datasets
│   ├── ObservableCollection<string> GeneratedQuestions
│   └── scalar properties (DatasetKey, Question, IsLoading, search toggles...)
├── SearchViewModel
│   └── ObservableCollection<AppSearchResult> Results
└── CommunityExplorerViewModel
    └── ObservableCollection<CommunityReport> Reports
```

### Data Flow

```
                    ┌──────────────┐
                    │ appsettings  │
                    │    .json     │
                    └──────┬───────┘
                           │ IOptions<SearchAppConfig>
                    ┌──────▼───────┐
                    │ DatasetLoader │──── listing.json ──── DatasetConfig[]
                    └──────┬───────┘
                           │ creates
              ┌────────────┴────────────┐
              │                         │
    ┌─────────▼─────────┐    ┌─────────▼─────────┐
    │ LocalFileDatasource│    │  BlobDatasource    │
    │ (Parquet/CSV)      │    │ (Azure Blob)       │
    └─────────┬──────────┘    └─────────┬──────────┘
              │                         │
              └────────────┬────────────┘
                           │ IDatasource
                    ┌──────▼───────────────┐
                    │ KnowledgeModelService │── loads in parallel
                    └──────┬───────────────┘
                           │ KnowledgeModel
                    ┌──────▼───────────────┐
                    │  AppStateViewModel   │
                    └──────┬───────────────┘
                           │
              ┌────────────┴────────────┐
              │                         │
    ┌─────────▼─────────┐    ┌─────────▼──────────────┐
    │   Search Page      │    │  Community Explorer     │
    │ → SearchOrchestrator│    │  → CommunityExplorerVM  │
    │ → SearchViewModel  │    └────────────────────────┘
    └────────────────────┘
```

---

## Extending the App

### Adding a New Page

1. Create `Pages/MyPage.razor` with a `@page "/mypage"` directive
2. Add navigation in `Layout/Sidebar.razor`:
   ```razor
   <MudNavLink Href="/mypage" Icon="@Icons.Material.Filled.Star">My Page</MudNavLink>
   ```
3. Create a ViewModel if needed (register as Scoped in `Program.cs`)

### Adding a New Data Source

1. Implement `IDatasource` (see `LocalFileDatasource.cs` for an example)
2. Update `DatasetLoader.CreateDatasource()` to select your new source based on configuration

### Customizing the Theme

MudBlazor themes can be configured in `Layout/MainLayout.razor`:

```razor
<MudThemeProvider Theme="_customTheme" />

@code {
    private MudTheme _customTheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1976d2",
            Secondary = "#dc004e",
        }
    };
}
```

---

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| **App won't start** | Verify .NET 10 SDK: `dotnet --version` should show `10.x.x` |
| **"No community reports loaded"** | Select a dataset in the sidebar; ensure `listing.json` and Parquet files exist |
| **Build error NU1507** | Ensure `dotnet/nuget.config` exists with `<clear />` to reset NuGet sources |
| **HTTPS certificate error** | Run `dotnet dev-certs https --trust` to trust the development certificate |
| **No datasets in dropdown** | Check that `listing.json` exists at `{DataRoot}/{ListingFile}` and is valid JSON |
| **Blob storage auth failure** | Run `az login` or set `AZURE_CLIENT_ID`/`AZURE_TENANT_ID`/`AZURE_CLIENT_SECRET` |
| **Search returns errors** | Ensure LLM/embedding services are configured and accessible |
| **Parquet read errors** | Verify files are valid Parquet format (not CSV renamed to .parquet) |
| **Docker build fails** | Ensure you run `docker build` from the `dotnet/` directory (not `src/GraphRag.SearchApp/`) |
| **Container can't read data** | Check the volume mount path and permissions; use `:ro` for read-only |
| **Docker port conflict** | Change the host port: `-p 9090:8080` or update `docker-compose.yml` |
| **Seed data not appearing** | Ensure `--seed` was passed; check that `DataRoot` matches where seed writes (default `./output`) |
| **Seed data overwrite** | Use `--seed --force` to regenerate; without `--force`, existing data is preserved |
| **Container re-seeds every time** | The volume may not be persisted; use a named Docker volume or bind mount |

### Checking Logs

The app logs to the console by default. Set logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "GraphRag.SearchApp": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Running Tests

The SearchApp has unit tests for ViewModels, services, and configuration:

```bash
cd dotnet
dotnet test tests/GraphRag.Tests.Unit --filter "SearchApp"
```

Current test coverage: **32 tests** covering ViewModels (property change notifications, ObservableCollections, filtering), SearchOrchestrator (parallel execution, error handling), MarkdownRenderer, and SearchAppConfig defaults.

---

## Further Reading

- [.NET Getting Started Guide](getting-started.md) — Full guide for the .NET GraphRAG CLI and libraries
- [.NET Project Overview](project-overview.md) — Complete breakdown of every project in the solution
- [Strategy Isolation Plan](strategy-isolation-plan.md) — Architecture of the plugin-based dependency isolation
- [Python unified-search-app README](../../unified-search-app/README.md) — Original Python/Streamlit app documentation
