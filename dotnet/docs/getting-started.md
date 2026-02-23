# Getting Started with .NET GraphRAG

A step-by-step guide to setting up, configuring, and running **GraphRAG for .NET** — a graph-based Retrieval-Augmented Generation system that builds knowledge graphs from your documents and uses them to power more accurate, context-rich LLM responses.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Installation](#installation)
3. [Quick Start](#quick-start)
4. [Project Structure](#project-structure)
5. [Configuration](#configuration)
6. [Strategy Plugins](#strategy-plugins)
7. [CLI Reference](#cli-reference)
8. [Integrating with an Agent](#integrating-with-an-agent)
9. [Programmatic API Usage](#programmatic-api-usage)
10. [Search Web App](#search-web-app)
11. [Troubleshooting](#troubleshooting)

---

## Prerequisites

| Requirement | Version | Notes |
|------------|---------|-------|
| **.NET SDK** | 10.0.100 or later | [Download .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **Azure OpenAI** (or OpenAI) | API key required | For LLM completions and embeddings |
| **Git** | Any recent version | To clone the repository |

Verify your .NET installation:

```bash
dotnet --version
# Expected: 10.0.100 or higher
```

---

## Installation

### Clone and Build

```bash
git clone https://github.com/microsoft/graphrag.git
cd graphrag/dotnet
dotnet build
```

The solution produces a CLI executable at `src/GraphRag/bin/Debug/net10.0/graphrag`.

### Run Tests (Optional)

```bash
dotnet test
# Expected: 200 tests passing (195 unit + 5 integration)
```

---

## Quick Start

### 1. Initialize a Project

Create a new GraphRAG project directory with default settings:

```bash
dotnet run --project src/GraphRag -- init --root ./my-project
```

This creates `./my-project/settings.yaml` with a minimal configuration template.

### 2. Add Your Documents

Place your source documents in the input directory:

```bash
mkdir ./my-project/input
cp /path/to/your/documents/*.txt ./my-project/input/
```

Supported input formats (with appropriate strategy plugins):

| Format | Built-in | Strategy Plugin Required |
|--------|----------|------------------------|
| Plain text (`.txt`) | ✅ | — |
| JSON (`.json`) | ✅ | — |
| JSON Lines (`.jsonl`) | ✅ | — |
| CSV (`.csv`) | ✅ (basic) | `GraphRag.Input.CsvHelper` for robust parsing |
| Markdown (`.md`) | — | `GraphRag.Input.Markdig` |
| Word (`.docx`) | — | `GraphRag.Input.OpenXml` |

### 3. Configure Your Environment

Create a `.env` file in your project root with your API credentials:

```bash
# ./my-project/.env
GRAPHRAG_API_KEY=your-azure-openai-api-key
GRAPHRAG_API_BASE=https://your-resource.openai.azure.com/
GRAPHRAG_API_VERSION=2024-06-01
```

Then update `settings.yaml` to reference them (see [Configuration](#configuration) below).

### 4. Build the Knowledge Graph Index

```bash
dotnet run --project src/GraphRag -- index --root ./my-project
```

This runs the full indexing pipeline:
1. **Input** — reads documents from the input directory
2. **Chunking** — splits text into processable segments
3. **Entity Extraction** — identifies entities and relationships using the LLM
4. **Graph Construction** — builds a knowledge graph from extracted data
5. **Community Detection** — clusters related entities into communities
6. **Summarization** — generates community reports and descriptions
7. **Embedding** — creates vector embeddings for search

Output is written to `./my-project/output/`.

### 5. Query the Knowledge Graph

```bash
# Local search — best for specific entity questions
dotnet run --project src/GraphRag -- query --root ./my-project --method local \
  --query "What are the main technologies discussed?"

# Global search — best for broad thematic questions
dotnet run --project src/GraphRag -- query --root ./my-project --method global \
  --query "What are the key themes across all documents?"

# DRIFT search — dynamic reasoning with iterative tree expansion
dotnet run --project src/GraphRag -- query --root ./my-project --method drift \
  --query "How do the organizations relate to each other?"

# Basic search — simple vector similarity search
dotnet run --project src/GraphRag -- query --root ./my-project --method basic \
  --query "Tell me about the recent events mentioned."
```

---

## Project Structure

After initialization, your project directory looks like this:

```
my-project/
├── .env                  ← API keys and secrets (gitignored)
├── settings.yaml         ← Main configuration file
├── input/                ← Source documents to index
├── output/               ← Generated index artifacts
│   ├── lancedb/          ← Vector store (default)
│   └── *.parquet         ← Graph data tables
├── cache/                ← LLM response cache (reduces API costs)
└── prompts/              ← Custom prompts (after prompt-tune)
```

### Solution Architecture

The .NET solution uses a **modular strategy pattern**:

```
┌──────────────────────────────────────────────────────────┐
│  GraphRag CLI (Console App)                              │
│  Orchestrates indexing, querying, and prompt-tuning       │
├──────────────────────────────────────────────────────────┤
│  Core Libraries (interfaces + built-in implementations)  │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────────┐  │
│  │ Storage  │ │  Vectors │ │   LLM    │ │   Input    │  │
│  │ IStorage │ │IVectorSt.│ │ILlmCompl.│ │IInputReader│  │
│  └──────────┘ └──────────┘ └──────────┘ └────────────┘  │
├──────────────────────────────────────────────────────────┤
│  Strategy Plugins (drop-in DLLs — one per 3rd-party dep) │
│  AzureBlob│AzureCosmos│AzureOpenAI│SharpToken│Scriban│… │
└──────────────────────────────────────────────────────────┘
```

Core libraries depend **only on interfaces** — no Azure SDKs, no NLP libraries. All 3rd-party integrations are isolated into separate strategy plugin assemblies that are discovered at runtime.

---

## Configuration

### settings.yaml

The main configuration file controls every aspect of the pipeline. Here is a complete reference:

```yaml
# ── LLM Models ──────────────────────────────────────────
# Define one or more completion models.
completion_models:
  default_completion_model:
    model: gpt-4.1                      # Model name / deployment ID
    provider: openai                     # "openai" or "azure_openai"
    api_key: ${GRAPHRAG_API_KEY}         # Environment variable substitution
    api_base: ${GRAPHRAG_API_BASE}       # Azure endpoint (if azure_openai)
    api_version: ${GRAPHRAG_API_VERSION} # Azure API version

# Define one or more embedding models.
embedding_models:
  default_embedding_model:
    model: text-embedding-3-large
    provider: openai
    api_key: ${GRAPHRAG_API_KEY}
    api_base: ${GRAPHRAG_API_BASE}

# ── Storage ─────────────────────────────────────────────
# Where documents are read from and results written to.
input_storage:
  type: file                             # "file" (built-in) or "blob", "cosmosdb"
  base_dir: input

output_storage:
  type: file
  base_dir: output

# ── Cache ───────────────────────────────────────────────
# Caches LLM responses to reduce API costs on re-runs.
cache:
  type: json                             # "json" (file-based), "memory", or "none"
  storage:
    base_dir: cache

# ── Vector Store ────────────────────────────────────────
# Where embeddings are stored for search.
vector_store:
  type: lancedb                          # "lancedb", "azure_ai_search", "cosmosdb"
  db_uri: output/lancedb

# ── Input ───────────────────────────────────────────────
input:
  type: text                             # "text", "csv", "json"
  file_pattern: ".*\\.txt$"

# ── Pipeline Tuning ─────────────────────────────────────
chunking:
  strategy: sentence                     # "sentence" or "tokens"
  size: 1200
  overlap: 100

extract_graph:
  max_gleanings: 1
  entity_types:
    - organization
    - person
    - geo
    - event

cluster_graph:
  strategy: leiden
  max_cluster_size: 10

# ── Search Configuration ────────────────────────────────
local_search:
  max_tokens: 12000
  top_k_entities: 10
  top_k_relationships: 10
  top_k_community_reports: 5

global_search:
  map_max_tokens: 8000
  reduce_max_tokens: 12000
  concurrency: 32

# ── Strategy Plugins ────────────────────────────────────
strategies:
  defaults:
    storage: file
    cache: json
    vector_store: lancedb
    completion: azure_openai
    embedding: azure_openai
    tokenizer: sharptoken
    template_engine: scriban
    table_provider: parquet
    input_reader: text
    noun_phrase_extractor: regex

  # Explicit assembly list (optional — auto-discovery also works).
  assemblies:
    - GraphRag.Llm.AzureOpenAi
    - GraphRag.Llm.SharpToken
    - GraphRag.Llm.Scriban
    - GraphRag.Storage.Parquet
```

### Environment Variable Substitution

Any value in `settings.yaml` can reference environment variables using `${VAR_NAME}` syntax. Variables are loaded from:

1. System environment variables
2. A `.env` file in the same directory as `settings.yaml`

```yaml
api_key: ${GRAPHRAG_API_KEY}
api_base: ${GRAPHRAG_API_BASE}
```

---

## Strategy Plugins

GraphRAG uses a **strategy pattern** to keep core libraries free of 3rd-party dependencies. Each external integration is a separate DLL that you include as needed.

### Available Plugins

| Plugin | NuGet Dependency | Interface | Strategy Key |
|--------|-----------------|-----------|-------------|
| **GraphRag.Storage.AzureBlob** | Azure.Storage.Blobs | `IStorage` | `blob` |
| **GraphRag.Storage.AzureCosmos** | Microsoft.Azure.Cosmos | `IStorage` | `cosmosdb` |
| **GraphRag.Storage.Parquet** | Parquet.Net | `ITableProvider` | `parquet` |
| **GraphRag.Storage.Csv** | CsvHelper | `ITableProvider` | `csv` |
| **GraphRag.Vectors.AzureAiSearch** | Azure.Search.Documents | `IVectorStore` | `azure_ai_search` |
| **GraphRag.Vectors.AzureCosmos** | Microsoft.Azure.Cosmos | `IVectorStore` | `cosmosdb` |
| **GraphRag.Vectors.LanceDb** | — (stub) | `IVectorStore` | `lancedb` |
| **GraphRag.Llm.AzureOpenAi** | Azure.AI.OpenAI | `ILlmCompletion`, `ILlmEmbedding` | `azure_openai` |
| **GraphRag.Llm.SharpToken** | SharpToken | `ITokenizer` | `sharptoken` |
| **GraphRag.Llm.Scriban** | Scriban | `ITemplateEngine` | `scriban` |
| **GraphRag.Input.CsvHelper** | CsvHelper | `IInputReader` | `csvhelper` |
| **GraphRag.Input.Markdig** | Markdig | `IInputReader` | `markdown` |
| **GraphRag.Input.OpenXml** | DocumentFormat.OpenXml | `IInputReader` | `openxml` |
| **GraphRag.Nlp.Catalyst** | Catalyst | `INounPhraseExtractor` | `catalyst` |
| **GraphRag.Graph.QuikGraph** | QuikGraph | `IGraphAlgorithms` | `quikgraph` |

### How Plugin Discovery Works

1. **Auto-Discovery**: At startup, all `GraphRag.*.dll` files in the application directory are scanned for classes decorated with `[StrategyImplementation]`.
2. **Explicit Configuration**: List assemblies in `settings.yaml` under `strategies.assemblies`.
3. **Drop-in Deployment**: Copy a strategy DLL into the app directory — no code changes needed.

### Adding a Plugin

To include a strategy plugin in your deployment:

```bash
# Build the strategy library
dotnet build src/GraphRag.Llm.AzureOpenAi

# Copy the DLL to the main app's output directory
cp src/GraphRag.Llm.AzureOpenAi/bin/Debug/net10.0/GraphRag.Llm.AzureOpenAi.dll \
   src/GraphRag/bin/Debug/net10.0/
```

Or add a project reference in `GraphRag.csproj` to include it automatically:

```xml
<ProjectReference Include="..\GraphRag.Llm.AzureOpenAi\GraphRag.Llm.AzureOpenAi.csproj" />
```

### Writing a Custom Strategy Plugin

Create a new class library and implement the target interface:

```csharp
using GraphRag.Common.Discovery;
using GraphRag.Storage;

namespace GraphRag.Storage.MyCustomBackend;

[StrategyImplementation("my_backend", typeof(IStorage))]
public sealed class MyCustomStorage : IStorage
{
    // Implement all IStorage methods...
    public async Task<object?> GetAsync(string key, string? encoding = null,
        CancellationToken cancellationToken = default)
    {
        // Your implementation here
    }

    // ... remaining interface methods
}
```

Build the DLL and drop it into the application directory. Then configure it:

```yaml
strategies:
  defaults:
    storage: my_backend
```

---

## CLI Reference

### `graphrag init`

Scaffolds a new project with default `settings.yaml`.

```
graphrag init [--root <path>]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--root` | `.` (current dir) | Directory to initialize |

### `graphrag index`

Runs the full indexing pipeline to build the knowledge graph.

```
graphrag index [--root <path>] [--method <method>] [--verbose]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--root` | `.` | Project root directory |
| `--method` | `standard` | Indexing method |
| `--verbose` | `false` | Enable verbose logging |

### `graphrag query`

Executes a search query against the knowledge graph.

```
graphrag query --method <method> --query "<text>" [--root <path>]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--method` | *(required)* | `local`, `global`, `drift`, or `basic` |
| `--query` | *(required)* | The natural language query |
| `--root` | `.` | Project root directory |

#### Search Methods

| Method | Best For | How It Works |
|--------|----------|-------------|
| **local** | Specific entity questions | Retrieves relevant entities and their neighborhoods from the graph, then uses the LLM to synthesize an answer from that focused context. |
| **global** | Broad thematic questions | Distributes the query across community reports using a map-reduce pattern, then reduces partial answers into a final comprehensive response. |
| **drift** | Complex relational questions | Uses Dynamic Reasoning with Iterative Fact Tree expansion — starts with local context and iteratively expands the search tree. |
| **basic** | Simple similarity lookup | Performs vector similarity search against embedded chunks without graph structure, similar to traditional RAG. |

### `graphrag prompt-tune`

Generates optimized prompts tailored to your specific dataset.

```
graphrag prompt-tune [--root <path>] [--output <dir>]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--root` | `.` | Project root directory |
| `--output` | `prompts` | Directory for generated prompts |

---

## Integrating with an Agent

GraphRAG is designed to serve as a **knowledge retrieval backend** for AI agents. There are several integration patterns depending on your agent framework.

### Pattern 1: Direct Library Reference

Add GraphRAG as a project dependency in your agent application and call the query engine directly.

```csharp
using GraphRag.Common.Config;
using GraphRag.Common.Discovery;
using GraphRag.Query;
using GraphRag.Startup;

// 1. Load configuration
var config = ConfigLoader.LoadConfig<GraphRagConfig>(
    data => new GraphRagConfig { /* map from dict */ },
    new ConfigLoaderOptions { ConfigPath = "./my-project" });

// 2. Discover strategy plugins
var discovery = StrategyRegistration.DiscoverStrategies();

// 3. Create search engine via factory
var queryFactory = new QueryFactory();
queryFactory.RegisterFromDiscovery(discovery);

var search = queryFactory.Create("local");

// 4. Execute queries from your agent
var result = await search.SearchAsync("What technologies are discussed?");
Console.WriteLine(result.Response);
```

### Pattern 2: Agent Tool / Function Calling

Expose GraphRAG as a tool that an LLM agent can invoke. This works with any framework supporting tool/function calling (Semantic Kernel, AutoGen, LangChain, etc.).

#### Semantic Kernel Example

```csharp
using Microsoft.SemanticKernel;

public class GraphRagPlugin
{
    private readonly ISearch _localSearch;
    private readonly ISearch _globalSearch;

    public GraphRagPlugin(ISearch localSearch, ISearch globalSearch)
    {
        _localSearch = localSearch;
        _globalSearch = globalSearch;
    }

    [KernelFunction("search_knowledge_graph_local")]
    [Description("Search the knowledge graph for specific entity information. " +
                 "Use for questions about specific people, organizations, or events.")]
    public async Task<string> LocalSearchAsync(
        [Description("The search query")] string query,
        CancellationToken ct = default)
    {
        var result = await _localSearch.SearchAsync(query, ct);
        return result.Response;
    }

    [KernelFunction("search_knowledge_graph_global")]
    [Description("Search the knowledge graph for broad thematic information. " +
                 "Use for questions about overarching themes or summaries.")]
    public async Task<string> GlobalSearchAsync(
        [Description("The search query")] string query,
        CancellationToken ct = default)
    {
        var result = await _globalSearch.SearchAsync(query, ct);
        return result.Response;
    }
}

// Register with your Semantic Kernel agent:
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

kernel.Plugins.AddFromObject(new GraphRagPlugin(localSearch, globalSearch));
```

#### AutoGen / Multi-Agent Example

```csharp
// Define GraphRAG as a tool the agent can call
var graphRagTool = new FunctionTool(
    name: "query_knowledge_graph",
    description: "Query a knowledge graph built from the organization's documents. " +
                 "Supports methods: local (entity-specific), global (thematic), " +
                 "drift (relational), basic (vector similarity).",
    parameters: new
    {
        method = "The search method: local, global, drift, or basic",
        query = "The natural language query"
    },
    handler: async (string method, string query) =>
    {
        var search = queryFactory.Create(method);
        var result = await search.SearchAsync(query);
        return result.Response;
    });

// Attach to your agent
agent.AddTool(graphRagTool);
```

### Pattern 3: REST API Wrapper

Wrap GraphRAG in a minimal ASP.NET Core API for language-agnostic agent integration:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register GraphRAG services
var discovery = StrategyRegistration.DiscoverStrategies();
builder.Services.AddSingleton(discovery);
builder.Services.AddSingleton<QueryFactory>();

var app = builder.Build();

app.MapPost("/query", async (QueryRequest request, QueryFactory factory) =>
{
    var search = factory.Create(request.Method);
    var result = await search.SearchAsync(request.Query);
    return Results.Ok(new { result.Response, result.Context });
});

app.Run();

record QueryRequest(string Method, string Query);
```

Then call from any agent (Python, TypeScript, etc.):

```python
import requests

response = requests.post("http://localhost:5000/query", json={
    "method": "local",
    "query": "What are the key technologies?"
})
print(response.json()["Response"])
```

### Pattern 4: MCP (Model Context Protocol) Server

For agents that support [MCP](https://modelcontextprotocol.io/), expose GraphRAG as an MCP resource and tool server:

```csharp
// MCP tool registration
[McpTool("graphrag_query")]
[Description("Query a knowledge graph using GraphRAG")]
public async Task<string> QueryAsync(
    [Description("Search method: local, global, drift, basic")] string method,
    [Description("The natural language query")] string query)
{
    var search = _queryFactory.Create(method);
    var result = await search.SearchAsync(query);
    return result.Response;
}

// MCP resource for exposing graph state
[McpResource("graphrag://index/status")]
public async Task<IndexStatus> GetIndexStatusAsync()
{
    return new IndexStatus
    {
        DocumentCount = await _storage.CountAsync(),
        LastIndexed = await _storage.GetLastModifiedAsync(),
    };
}
```

The MCP server can push notifications to connected agents when the index is updated using SSE-based `notifications/resources/updated` events.

### Best Practices for Agent Integration

1. **Pre-build the index** — Run `graphrag index` before deploying the agent. Indexing is expensive and should be done offline.
2. **Choose the right search method** — Use `local` for entity-specific questions, `global` for thematic summaries, `drift` for complex relational queries.
3. **Cache aggressively** — Set `cache.type: json` to avoid re-calling the LLM for identical queries during development.
4. **Provide search method guidance** — In your tool descriptions, tell the agent when to use each search method.
5. **Return structured context** — Include the `SearchResult.Context` in your agent's tool response so the LLM can reason about source provenance.

---

## Programmatic API Usage

### Key Interfaces

| Interface | Purpose | Namespace |
|-----------|---------|-----------|
| `IStorage` | Read/write documents and data | `GraphRag.Storage` |
| `IVectorStore` | Store and search vector embeddings | `GraphRag.Vectors` |
| `ILlmCompletion` | LLM chat completions | `GraphRag.Llm` |
| `ILlmEmbedding` | Text → vector embeddings | `GraphRag.Llm` |
| `ITokenizer` | Token counting and encoding | `GraphRag.Llm` |
| `ITemplateEngine` | Prompt template rendering | `GraphRag.Llm` |
| `IInputReader` | Document ingestion | `GraphRag.Input` |
| `ICache` | Response caching | `GraphRag.Cache` |
| `ISearch` | Query execution | `GraphRag.Query` |

### Factory Pattern

All implementations are created through factories that support configuration-driven strategy selection:

```csharp
using GraphRag.Storage;

var factory = new StorageFactory();

// Register built-in strategies
factory.Register("file", args => new FileStorage(
    args.GetValueOrDefault("base_dir")?.ToString() ?? "output"));
factory.Register("memory", _ => new MemoryStorage());

// Auto-register discovered plugin strategies (e.g., AzureBlob, Cosmos)
factory.RegisterFromDiscovery(discovery);

// Create an instance by strategy name
IStorage storage = factory.Create("file", new Dictionary<string, object?>
{
    ["base_dir"] = "./my-project/output",
});
```

---

## Search Web App

The **GraphRag.SearchApp** is a Blazor Server web application that provides an interactive UI for searching and exploring your GraphRAG indexed data. It is the .NET equivalent of the Python/Streamlit `unified-search-app`.

> **📖 Full guide**: See the dedicated [Search App Getting Started Guide](search-app-getting-started.md) for complete setup, configuration, data preparation, and usage instructions.

### Quick Start

```bash
cd dotnet/src/GraphRag.SearchApp
dotnet run
# Open https://localhost:5001
```

### Configuration

Edit `appsettings.json` to point to your indexed data:

```json
{
  "SearchApp": {
    "DataRoot": "./output",
    "ListingFile": "listing.json",
    "DefaultSuggestedQuestions": 5,
    "BlobAccountName": "",
    "BlobContainerName": ""
  }
}
```

- **DataRoot**: Local directory containing indexed output and `listing.json`
- **BlobAccountName/BlobContainerName**: Set both to read datasets from Azure Blob Storage instead of local files
- **ListingFile**: JSON file listing available datasets (array of `{ key, path, name, description, communityLevel }` objects)

### Features

- **Search**: Run Global, Local, DRIFT, and Basic RAG searches in parallel
- **Community Explorer**: Browse community reports filtered by level
- **Dataset switching**: Select from multiple datasets via the sidebar
- **Markdown rendering**: LLM responses rendered as formatted HTML

---

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| `dotnet build` fails with NU1507 | Ensure `nuget.config` exists with `<clear />` to reset package sources |
| Tests show "Test Run Aborted" | This is a known `.slnx` cosmetic issue — check individual test results (they pass) |
| `Environment variable not found` | Create a `.env` file or set variables before running |
| Plugin DLL not found | Ensure the strategy DLL is in the same directory as `graphrag.dll` |
| `Strategy 'X' is not registered` | Check that the plugin assembly is built and present in the app directory |

### Logging

Enable verbose output for debugging:

```bash
dotnet run --project src/GraphRag -- index --root ./my-project --verbose
```

### Resetting State

To re-index from scratch, delete the output and cache directories:

```bash
rm -rf ./my-project/output ./my-project/cache
dotnet run --project src/GraphRag -- index --root ./my-project
```

---

## Next Steps

- **[Project Overview](project-overview.md)** — Detailed breakdown of all 23 projects in the solution
- **[Strategy Isolation Plan](strategy-isolation-plan.md)** — Architecture of the plugin system
- **[Microsoft GraphRAG Paper](https://arxiv.org/abs/2404.16130)** — The research behind GraphRAG
