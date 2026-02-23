# Getting Started with the GraphRAG Search App

A step-by-step guide to setting up, configuring, and running the **GraphRAG Blazor Search App** — an interactive web UI for searching and exploring GraphRAG-indexed knowledge graphs.

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Build & Run](#build--run)
4. [Docker](#docker)
5. [Preparing Your Data](#preparing-your-data)
6. [Configuration Reference](#configuration-reference)
7. [Using the App](#using-the-app)
8. [Data Sources](#data-sources)
9. [Architecture](#architecture)
10. [Extending the App](#extending-the-app)
11. [Troubleshooting](#troubleshooting)

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

# Place your indexed data in ./output/ (or change the volume mount)
mkdir -p output

# Build and start
docker compose up --build
```

The app is available at `http://localhost:8080`.

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
| **Entry point** | `uv run poe start_prod` | `dotnet GraphRag.SearchApp.dll` |
| **Data mount** | N/A (env vars) | `/data/output` volume |
| **Non-root user** | No | Yes (`appuser`) |

---

## Preparing Your Data

The Search App reads data produced by a GraphRAG indexing run. You need:

1. **Indexed output files** — Parquet files generated by `graphrag index`
2. **A listing file** — JSON file describing available datasets

### Expected Output Files

After running `graphrag index`, your output directory should contain:

```
output/
├── entities.parquet             # Extracted entities
├── relationships.parquet        # Entity relationships
├── communities.parquet          # Detected communities
├── community_reports.parquet    # Generated community reports
├── text_units.parquet           # Source text chunks
└── covariates.parquet           # (optional) Extracted covariates
```

### Create a Dataset Listing File

Create a `listing.json` file in your data root directory:

```json
[
  {
    "key": "my-dataset",
    "path": "my-dataset/output",
    "name": "My Dataset",
    "description": "Knowledge graph built from my documents",
    "communityLevel": 0
  },
  {
    "key": "another-dataset",
    "path": "another-dataset/output",
    "name": "Another Dataset",
    "description": "A second indexed dataset",
    "communityLevel": 2
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

```
./output/                          ← DataRoot
├── listing.json                   ← Dataset listing
├── my-dataset/
│   └── output/
│       ├── entities.parquet
│       ├── relationships.parquet
│       ├── communities.parquet
│       ├── community_reports.parquet
│       └── text_units.parquet
└── another-dataset/
    └── output/
        ├── entities.parquet
        └── ...
```

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

1. **Select a dataset** from the sidebar dropdown
2. **Choose search types** using the sidebar toggles:
   - **Global** (on by default) — Community-based global search
   - **Local** (on by default) — Local context search using entity neighborhoods
   - **DRIFT** (off by default) — Dynamic retrieval with iterative feedback
   - **Basic RAG** (off by default) — Traditional vector-similarity search
3. **Type your question** in the search bar and press Enter or click Search
4. **View results** — Each enabled search type runs in parallel and displays in its own card with:
   - Markdown-rendered LLM response
   - Performance stats: completion time, LLM calls, token usage
   - Search type icon and label

**Suggested Questions**: When available, clickable question chips appear above the results. Click any chip to auto-fill and execute that question.

### Community Explorer (`/communities`)

The Community Explorer provides a split-panel view for browsing community reports.

1. **Left panel** — Scrollable list of community reports
   - **Filter by level** using the dropdown at the top
   - Click any report to select it
2. **Right panel** — Full detail view of the selected report showing:
   - Title, community ID, rank, and size
   - Summary text
   - Full Markdown-rendered report content

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
└── appsettings.Development.json   # Development overrides
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
