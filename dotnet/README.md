# GraphRAG for .NET

A .NET 10 port of [Microsoft GraphRAG](https://github.com/microsoft/graphrag) — a graph-based retrieval-augmented generation (RAG) system that uses knowledge graphs to enhance LLM reasoning over complex datasets.

## Solution Structure

| Project | Type | Description |
|---------|------|-------------|
| `GraphRag` | Console App | CLI entry point — orchestrates indexing, querying, and prompt-tuning workflows |
| `GraphRag.Common` | Library | Shared abstractions, configuration loading, hashing utilities, and DI helpers |
| `GraphRag.Cache` | Library | Caching layer with memory, JSON file, and no-op implementations |
| `GraphRag.Chunking` | Library | Text chunking strategies (sentence-based, token-based) for document processing |
| `GraphRag.Input` | Library | Document ingestion — reads CSV, JSON, Markdown, DOCX, and plain text formats |
| `GraphRag.Llm` | Library | Azure OpenAI integration for completions, embeddings, tokenization, and templating |
| `GraphRag.Storage` | Library | Data persistence with Azure Blob, Cosmos DB, file system, and memory backends |
| `GraphRag.Vectors` | Library | Vector store abstraction with Azure AI Search and Cosmos DB support |

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

The solution follows a modular plugin architecture with factory patterns enabling configuration-driven selection of implementations:

- **Input** → Documents are read and normalized via `IInputReader`
- **Chunking** → Text is split into manageable chunks for processing
- **LLM** → Azure OpenAI handles completions, embeddings, and tokenization
- **Storage** → Indexed graphs and documents are persisted via `IStorage`
- **Vectors** → Embeddings are stored and searched via `IVectorStore`
- **Cache** → Intermediate results are cached to reduce redundant API calls

All configuration is driven by YAML/JSON settings files created during `init`.

## License

[MIT](../LICENSE)
