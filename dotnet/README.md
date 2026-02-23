# GraphRAG for .NET

A .NET 10 port of [Microsoft GraphRAG](https://github.com/microsoft/graphrag) — a graph-based retrieval-augmented generation (RAG) system that uses knowledge graphs to enhance LLM reasoning over complex datasets.

## Solution Structure

| Project | Type | Description |
|---------|------|-------------|
| `GraphRag` | Console App | CLI entry point — orchestrates indexing, querying, and prompt-tuning workflows |
| `GraphRag.Common` | Library | Shared abstractions, configuration loading, hashing utilities, strategy discovery, and DI helpers |
| `GraphRag.Cache` | Library | Caching layer with memory, JSON file, and no-op implementations |
| `GraphRag.Chunking` | Library | Text chunking strategies (sentence-based, token-based) for document processing |
| `GraphRag.Input` | Library | Document ingestion — reads CSV, JSON, and plain text formats (core) |
| `GraphRag.Llm` | Library | LLM abstraction layer for completions, embeddings, tokenization, and templating |
| `GraphRag.Storage` | Library | Data persistence with file system and memory backends (core) |
| `GraphRag.Vectors` | Library | Vector store abstraction with filtering and search |

### Strategy Libraries (Plugin Assemblies)

| Project | NuGet Dependency | Implements |
|---------|-----------------|------------|
| `GraphRag.Storage.AzureBlob` | Azure.Storage.Blobs | `IStorage` |
| `GraphRag.Storage.AzureCosmos` | Microsoft.Azure.Cosmos | `IStorage` |
| `GraphRag.Storage.Parquet` | Parquet.Net | `ITableProvider` |
| `GraphRag.Storage.Csv` | CsvHelper | `ITableProvider` |
| `GraphRag.Vectors.AzureAiSearch` | Azure.Search.Documents | `IVectorStore` |
| `GraphRag.Vectors.AzureCosmos` | Microsoft.Azure.Cosmos | `IVectorStore` |
| `GraphRag.Vectors.LanceDb` | (stub) | `IVectorStore` |
| `GraphRag.Llm.AzureOpenAi` | Azure.AI.OpenAI | `ILlmCompletion`, `ILlmEmbedding` |
| `GraphRag.Llm.SharpToken` | SharpToken | `ITokenizer` |
| `GraphRag.Llm.Scriban` | Scriban | `ITemplateEngine` |
| `GraphRag.Input.CsvHelper` | CsvHelper | `IInputReader` |
| `GraphRag.Input.Markdig` | Markdig | `IInputReader` |
| `GraphRag.Input.OpenXml` | DocumentFormat.OpenXml | `IInputReader` |
| `GraphRag.Nlp.Catalyst` | Catalyst | `INounPhraseExtractor` |
| `GraphRag.Graph.QuikGraph` | QuikGraph | `IGraphAlgorithms` |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (v10.0.100 or later)

## Build

```bash
dotnet build
```

## Test

```bash
# Run all tests
dotnet test

# Unit tests only
dotnet test tests/GraphRag.Tests.Unit

# Integration tests only
dotnet test tests/GraphRag.Tests.Integration
```

## Run the CLI

```bash
dotnet run --project src/GraphRag -- <command> [options]
```

### CLI Commands

| Command | Description |
|---------|-------------|
| `init` | Initialize a new GraphRAG project with default configuration |
| `index` | Build a knowledge graph index from input documents |
| `query` | Query the knowledge graph using natural language |
| `prompt-tune` | Auto-tune prompts for your specific dataset |

### Examples

```bash
# Initialize a new project
dotnet run --project src/GraphRag -- init --root ./my-project

# Build an index
dotnet run --project src/GraphRag -- index --root ./my-project

# Query the graph
dotnet run --project src/GraphRag -- query --root ./my-project --query "What are the main themes?"
```

## Architecture

The solution follows a **strategy pattern** architecture where core libraries contain only interfaces and pure-.NET implementations, while all 3rd-party integrations (Azure SDKs, NLP libraries, etc.) are isolated into separate plugin assemblies:

- **Core libraries** depend only on abstractions — no 3rd-party NuGet packages
- **Strategy libraries** each encapsulate one external dependency behind a core interface
- **Runtime discovery** auto-scans `GraphRag.*.dll` assemblies for `[StrategyImplementation]`-decorated classes
- **Configuration** maps strategy keys to implementations in `settings.yaml`

### Pipeline

- **Input** → Documents are read and normalized via `IInputReader`
- **Chunking** → Text is split into manageable chunks for processing
- **LLM** → Completions, embeddings, and tokenization via `ILlmCompletion`/`ILlmEmbedding`
- **Storage** → Indexed graphs and documents are persisted via `IStorage`
- **Vectors** → Embeddings are stored and searched via `IVectorStore`
- **Cache** → Intermediate results are cached to reduce redundant API calls

All configuration is driven by YAML/JSON settings files created during `init`.

## License

[MIT](../LICENSE)
