# GraphRAG .NET Solution — Project Overview

> A complete breakdown of every project in the solution: its purpose, output type, dependencies, exported types, and role in the overall architecture.

---

## Solution Structure

```
dotnet/
├── src/
│   ├── GraphRag.Common/        ← Shared foundations + strategy discovery
│   ├── GraphRag.Storage/       ← Storage interfaces + file/memory implementations
│   ├── GraphRag.Cache/         ← LLM response caching
│   ├── GraphRag.Chunking/      ← Document text splitting
│   ├── GraphRag.Input/         ← Document ingestion (core readers)
│   ├── GraphRag.Llm/           ← LLM abstraction layer (interfaces only)
│   ├── GraphRag.Vectors/       ← Vector store & filtering (interfaces only)
│   ├── GraphRag/               ← Main executable (CLI + orchestration)
│   │
│   │   Strategy Libraries (plugin assemblies):
│   ├── GraphRag.Storage.AzureBlob/    ← Azure Blob IStorage
│   ├── GraphRag.Storage.AzureCosmos/  ← Cosmos DB IStorage
│   ├── GraphRag.Storage.Parquet/      ← Parquet ITableProvider
│   ├── GraphRag.Storage.Csv/          ← CsvHelper ITableProvider
│   ├── GraphRag.Vectors.AzureAiSearch/ ← Azure AI Search IVectorStore
│   ├── GraphRag.Vectors.AzureCosmos/  ← Cosmos DB IVectorStore
│   ├── GraphRag.Vectors.LanceDb/     ← LanceDB IVectorStore (stub)
│   ├── GraphRag.Llm.AzureOpenAi/     ← Azure OpenAI ILlmCompletion/ILlmEmbedding
│   ├── GraphRag.Llm.SharpToken/      ← SharpToken ITokenizer
│   ├── GraphRag.Llm.Scriban/         ← Scriban ITemplateEngine
│   ├── GraphRag.Input.CsvHelper/     ← CsvHelper IInputReader
│   ├── GraphRag.Input.Markdig/       ← Markdig IInputReader
│   ├── GraphRag.Input.OpenXml/       ← OpenXml IInputReader
│   ├── GraphRag.Nlp.Catalyst/        ← Catalyst INounPhraseExtractor
│   └── GraphRag.Graph.QuikGraph/     ← QuikGraph IGraphAlgorithms
│   │
│   │   Web Application:
│   └── GraphRag.SearchApp/           ← Blazor Server search UI (MudBlazor)
├── tests/
│   ├── GraphRag.Tests.Unit/        ← 274 unit tests
│   └── GraphRag.Tests.Integration/ ← 5 integration tests
└── docs/
```

### Dependency Graph

```
GraphRag (Exe)
 ├── GraphRag.Llm
 │    ├── GraphRag.Common
 │    └── GraphRag.Cache
 │         ├── GraphRag.Common
 │         └── GraphRag.Storage
 │              └── GraphRag.Common
 ├── GraphRag.Vectors
 │    └── GraphRag.Common
 ├── GraphRag.Chunking
 │    └── GraphRag.Common
 ├── GraphRag.Input
 │    ├── GraphRag.Common
 │    └── GraphRag.Storage
 ├── GraphRag.Storage
 │    └── GraphRag.Common
 └── GraphRag.Cache
      ├── GraphRag.Common
      └── GraphRag.Storage

GraphRag.SearchApp (Blazor Server)
 ├── GraphRag.Common
 ├── GraphRag.Storage
 ├── GraphRag.Llm
 └── GraphRag (reference-only)
```

---

## 1. GraphRag.Common

| Attribute | Value |
|-----------|-------|
| **Output** | Class library (DLL) |
| **Project References** | None (leaf dependency) |
| **Role** | Core abstractions reused by every other project |

### Purpose

Provides the foundational building blocks for the entire solution:

- **`ServiceFactory<T>`** — Abstract generic factory base class implementing the strategy/factory pattern with hash-based singleton caching and transient scoping.
- **`HashHelper`** — SHA-256 hashing with YAML serialization for producing deterministic cache keys from complex objects.
- **`ConfigLoader`** — YAML/JSON configuration loading with environment variable substitution (`${ENV_VAR}`) and `.env` file support.
- **`ServiceScope`** — Enum controlling factory instance lifetime: `Transient` (new each time) or `Singleton` (cached by hash key).

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Common.Factories` | `ServiceFactory<T>` (abstract class), `ServiceScope` (enum) |
| `GraphRag.Common.Hasher` | `HashHelper` (static class) |
| `GraphRag.Common.Config` | `ConfigLoader` (static class), `ConfigParsingError` (exception) |

---

## 2. GraphRag.Storage

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common |
| **Role** | Abstracts file/blob/memory storage behind a uniform interface |

### Purpose

Provides a pluggable storage abstraction for reading/writing data by key. The `IStorage` interface supports file finding via regex, async get/set/has/delete/clear operations, hierarchical child storage, and creation date retrieval. The `Tables/` subsystem adds row-oriented access for Parquet and CSV formats.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Storage` | `IStorage` (interface), `FileStorage` (class), `MemoryStorage` (class), `StorageFactory` (class), `StorageType` (static constants), `StorageConfig` (sealed record) |
| `GraphRag.Storage.Tables` | `ITable` (interface), `ITableProvider` (interface), `TableProviderFactory` (class), `TableType` (static constants), `TableProviderConfig` (sealed record) |

### Implementations

| Implementation | Backend | 3rd-Party Deps |
|---------------|---------|----------------|
| `FileStorage` | `System.IO` async APIs | None (pure .NET) |
| `MemoryStorage` | `ConcurrentDictionary<string, object>` | None (pure .NET) |
| Azure Blob | Stub (`NotImplementedException`) | Azure.Storage.Blobs (declared) |
| Azure Cosmos | Stub (`NotImplementedException`) | Microsoft.Azure.Cosmos (declared) |

---

## 3. GraphRag.Cache

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common, GraphRag.Storage |
| **Role** | Caches expensive LLM responses to avoid redundant API calls |

### Purpose

Provides a caching layer with three strategies: JSON-file persistence (backed by any `IStorage`), in-memory dict, and no-op passthrough. `CacheKeyHelper` produces deterministic hash keys from request arguments for cache lookup.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Cache` | `ICache` (interface), `JsonCache` (class), `MemoryCache` (class), `NoopCache` (class), `CacheFactory` (class), `CacheType` (static constants), `CacheConfig` (sealed record), `CacheKeyHelper` (static class) |

### Implementations

| Implementation | Backend | 3rd-Party Deps |
|---------------|---------|----------------|
| `JsonCache` | Wraps any `IStorage` with JSON serialization | None (uses `System.Text.Json`) |
| `MemoryCache` | `ConcurrentDictionary<string, object?>` | None |
| `NoopCache` | No-op (get→null, has→false, child→self) | None |

---

## 4. GraphRag.Chunking

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common |
| **Role** | Splits long documents into overlapping chunks for LLM processing |

### Purpose

Implements text chunking strategies that break documents into token- or sentence-bounded chunks with configurable size and overlap. The `MetadataTransformer` prepends/appends key-value metadata to chunk text. Each chunk is represented as a `TextChunk` record carrying original text, transformed text, character offsets, and optional token count.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Chunking` | `IChunker` (interface), `TokenChunker` (class), `SentenceChunker` (class), `ChunkerFactory` (class), `ChunkerType` (static constants), `ChunkingConfig` (sealed record), `TextChunk` (sealed record), `ChunkResultHelper` (static class), `MetadataTransformer` (static class) |

### Implementations

| Implementation | Algorithm | 3rd-Party Deps |
|---------------|-----------|----------------|
| `TokenChunker` | Encode→slice with overlap→decode | None (caller provides encode/decode) |
| `SentenceChunker` | `[GeneratedRegex]` sentence boundary detection | None |

---

## 5. GraphRag.Input

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common, GraphRag.Storage |
| **Role** | Reads input documents from storage in various formats |

### Purpose

Provides format-specific file readers that load documents from an `IStorage` backend and produce `TextDocument` records. Each reader handles pattern-matched files (e.g., `.*\.csv$`), extracts text/title/ID, and generates SHA-512 hash IDs when no ID column is specified.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Input` | `IInputReader` (interface), `TextFileReader` (class), `CsvFileReader` (class), `JsonFileReader` (class), `JsonLinesFileReader` (class), `StructuredFileReader` (abstract class), `InputReaderFactory` (class), `InputType` (static constants), `InputConfig` (sealed record), `TextDocument` (sealed record), `PropertyHelper` (static class) |

### Implementations

| Implementation | Format | 3rd-Party Deps |
|---------------|--------|----------------|
| `TextFileReader` | `.txt` files → one document per file | None |
| `CsvFileReader` | `.csv` files → one document per row | CsvHelper (declared in csproj) |
| `JsonFileReader` | `.json` files → array or single object | None (`System.Text.Json`) |
| `JsonLinesFileReader` | `.jsonl` files → one object per line | None (`System.Text.Json`) |

---

## 6. GraphRag.Llm

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common, GraphRag.Cache |
| **Role** | Full LLM orchestration layer with pluggable middleware |

### Purpose

The largest supporting package. Provides abstractions for LLM completions, embeddings, tokenization, rate limiting, retries, metrics, and templating. The middleware pipeline chains cross-cutting concerns (logging → metrics → rate limiting → retries → cache) around base LLM calls. Nine factory classes create all component types from configuration.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Llm` | `ILlmCompletion`, `ILlmEmbedding`, `ITokenizer`, `IRateLimiter`, `IRetry`, `IMetricsStore`, `IMetricsWriter`, `IMetricsProcessor`, `ITemplateEngine`, `ITemplateManager` (all interfaces) |
| `GraphRag.Llm.Types` | `LlmCompletionResponse`, `LlmEmbeddingResponse`, `LlmCompletionArgs`, `LlmEmbeddingArgs`, `LlmMessage`, `LlmUsage`, `MetricsHelper` (sealed records + static class) |
| `GraphRag.Llm.Config` | `ModelConfig`, `MetricsConfig`, `RateLimitConfig`, `RetryConfig`, `TemplateEngineConfig`, `TokenizerConfig` (sealed records); 10 type-constant classes |
| `GraphRag.Llm.Completion` | `MockLlmCompletion` |
| `GraphRag.Llm.Embedding` | `MockLlmEmbedding` |
| `GraphRag.Llm.Tokenizer` | `SimpleTokenizer` |
| `GraphRag.Llm.Templating` | `SimpleTemplateEngine`, `FileTemplateManager` |
| `GraphRag.Llm.RateLimit` | `SlidingWindowRateLimiter` |
| `GraphRag.Llm.Retry` | `ExponentialRetry`, `ImmediateRetry` |
| `GraphRag.Llm.Metrics` | `MemoryMetricsStore`, `NoopMetricsStore`, `LogMetricsWriter`, `FileMetricsWriter`, `DefaultMetricsProcessor` |
| `GraphRag.Llm.Middleware` | `MiddlewarePipeline` (static) |
| `GraphRag.Llm.Cache` | `LlmCacheKeyCreator` (static) |
| `GraphRag.Llm.Factories` | 9 factory classes |
| `GraphRag.Llm.Threading` | `CompletionBatchRunner` (static) |

### Implementations

| Implementation | Purpose | 3rd-Party Deps |
|---------------|---------|----------------|
| `MockLlmCompletion` | Cycles through mock responses | None |
| `MockLlmEmbedding` | Seeded random embeddings | None |
| `SimpleTokenizer` | Whitespace split/join | None |
| `SimpleTemplateEngine` | `{{variable}}` replacement | None |
| `FileTemplateManager` | Reads `.txt` from disk | None |
| `SlidingWindowRateLimiter` | SemaphoreSlim + timestamp queue | None |
| `ExponentialRetry` | Backoff with jitter | None |
| `ImmediateRetry` | Retry without delay | None |
| `MemoryMetricsStore` | ConcurrentDictionary | None |
| `NoopMetricsStore` | No-op | None |
| `LogMetricsWriter` | ILogger-based | **Microsoft.Extensions.Logging** |
| `FileMetricsWriter` | JSON to disk | None |
| `DefaultMetricsProcessor` | Store + optional write | None |

---

## 7. GraphRag.Vectors

| Attribute | Value |
|-----------|-------|
| **Output** | Class library |
| **Project References** | GraphRag.Common |
| **Role** | Abstracts vector similarity search with a rich filtering system |

### Purpose

Provides `IVectorStore` for vector database operations (connect, create index, insert, similarity search by vector/text, search by ID, count, remove, update). Includes a composable filtering DSL with `Condition`, `AndExpression`, `OrExpression`, `NotExpression`, and the fluent `FieldRef` builder. `TimestampHelper` explodes ISO dates into searchable year/month/day/hour fields for vector store indexing.

### Exported Types

| Namespace | Types |
|-----------|-------|
| `GraphRag.Vectors` | `IVectorStore` (interface), `VectorStoreDocument` (sealed record), `VectorStoreSearchResult` (sealed record), `VectorStoreConfig` (sealed record), `IndexSchema` (sealed record), `VectorStoreType` (static constants), `VectorStoreFactory` (class), `TimestampHelper` (static class) |
| `GraphRag.Vectors.Filtering` | `FilterExpression` (abstract), `Condition` (class), `AndExpression` (class), `OrExpression` (class), `NotExpression` (class), `FieldRef` (class), `FieldBuilder` (static class), `ComparisonOperator` (static constants) |

### Implementations

| Implementation | Backend | 3rd-Party Deps |
|---------------|---------|----------------|
| LanceDB | Stub (`NotImplementedException`) | — (would need LanceDB client) |
| Azure AI Search | Stub (`NotImplementedException`) | Azure.Search.Documents (declared) |
| CosmosDB | Stub (`NotImplementedException`) | Microsoft.Azure.Cosmos (declared) |

---

## 8. GraphRag (Main Executable)

| Attribute | Value |
|-----------|-------|
| **Output** | **Console executable (Exe)** |
| **Project References** | All 7 libraries above |
| **Role** | Entry point: CLI, pipeline orchestration, query engine, data model, config |

### Purpose

The main application assembling all libraries into a working system. Contains the data model, configuration, indexing pipeline, query engine, prompt tuning, CLI commands, and embedded prompt templates.

### Subsystems

#### DataModel/
Core graph entities as sealed records with `Identified` → `Named` inheritance hierarchy.

| Type | Extends | Key Fields |
|------|---------|------------|
| `Entity` | `Named` | Type, Description, DescriptionEmbedding, CommunityIds, TextUnitIds, Rank |
| `Relationship` | `Identified` | Source, Target, Weight, Description, TextUnitIds, Rank |
| `Community` | `Named` | Level, Parent, Children, EntityIds, RelationshipIds, Size, Period |
| `CommunityReport` | `Named` | CommunityId, Summary, FullContent, Rank, FullContentEmbedding |
| `TextUnit` | `Identified` | Text, EntityIds, RelationshipIds, NTokens, DocumentId |
| `Document` | `Named` | Type, TextUnitIds, Text |
| `Covariate` | `Identified` | SubjectId, SubjectType, CovariateType, TextUnitIds |

#### Config/
Full configuration system: 16 config models, 6 enum classes, `GraphRagConfig` master record, `DefaultValues` constants. Matches every default value from the Python original.

#### Index/ (Indexing Pipeline)
| Component | Purpose |
|-----------|---------|
| `Pipeline` | Ordered list of `(name, WorkflowFunction)` tuples |
| `PipelineRunner` | Async executor yielding `PipelineRunResult` per workflow |
| `PipelineFactory` | Creates Standard / Fast / Update pipelines |
| `WorkflowProfiler` | Stopwatch-based per-workflow timing |
| `WorkflowRegistry` | Maps 22 workflow names to stub functions |
| `WorkflowNames` | 22 string constants for workflow identifiers |
| 7 operation interfaces | `IGraphExtractor`, `IClaimExtractor`, `ISummarizeExtractor`, `ICommunityReportsExtractor`, `ITextEmbedder`, `INounPhraseExtractor`, `GraphUtils` |

#### Query/ (Query Engine)
Four search strategies, all implementing `ISearch`:

| Strategy | Pattern | Context Builder |
|----------|---------|-----------------|
| `GlobalSearch` | Map-reduce across community reports | `IGlobalContextBuilder` |
| `LocalSearch` | Entity-centric with mixed context | `ILocalContextBuilder` |
| `BasicSearch` | Vector similarity + LLM | `IBasicContextBuilder` |
| `DriftSearch` | Iterative multi-hop refinement | `IDriftContextBuilder` |

Plus: `SearchResult`, `ConversationHistory`, `QaTurn`, `QueryFactory`, `IndexerAdapters`, `DriftAction`, `QueryState`.

#### Callbacks/
`IWorkflowCallbacks`, `IQueryCallbacks`, `ConsoleWorkflowCallbacks`, `WorkflowCallbacksManager` (composite), Noop variants.

#### PromptTune/
`PromptTuner` orchestrator, 8 generator classes (domain, persona, language, entity types, etc.), `DocumentLoader`, 3 prompt creators.

#### Cli/
System.CommandLine subcommands: `init`, `index`, `query`, `prompt-tune` via `RootCommandBuilder`.

#### Prompts/
10 embedded `.txt` templates (extraction, summarization, community reporting, search).

---

## Test Projects

### GraphRag.Tests.Unit

| Attribute | Value |
|-----------|-------|
| **Output** | Test library |
| **Tests** | **181** |
| **References** | All 8 src projects |
| **Purpose** | Unit tests for every source project — isolated class tests with mocks |

### GraphRag.Tests.Integration

| Attribute | Value |
|-----------|-------|
| **Output** | Test library |
| **Tests** | **5** |
| **References** | All 8 src projects |
| **Purpose** | Cross-cutting tests: FileStorage round-trips, JsonCache+FileStorage persistence, Pipeline execution with stub workflows, TokenChunker on large documents |

---

## GraphRag.SearchApp

| Attribute | Value |
|-----------|-------|
| **Output** | Blazor Server web application |
| **Project References** | GraphRag.Common, GraphRag.Storage, GraphRag.Llm, GraphRag (ref-only) |
| **NuGet** | MudBlazor 8.5.0, Markdig 0.38.0, Parquet.Net 5.3.0, Azure.Identity, Azure.Storage.Blobs |
| **Role** | Interactive web UI for searching and exploring GraphRAG indexed data |

> **📖 Full setup guide**: [Search App Getting Started](search-app-getting-started.md)

### Purpose
A .NET port of the Python/Streamlit `unified-search-app`. Provides:
- **Search page**: Run Global, Local, DRIFT, and Basic RAG searches in parallel with Markdown-rendered results and stats
- **Community Explorer**: Browse and filter community reports with detail view
- **Dataset management**: Switch between datasets via sidebar; supports local parquet and Azure Blob sources
- MVVM architecture with `ObservableCollection<T>` on all list properties

### Key Types
- `Config/SearchAppConfig` — `IOptions<T>` configuration (DataRoot, Blob settings, listing file)
- `Config/DatasetConfig` — Dataset descriptor record
- `Models/SearchType` — Enum: Basic, Local, Global, Drift
- `Models/AppSearchResult` — Search result wrapper with stats
- `Models/KnowledgeModel` — Loaded dataset (entities, relationships, communities, reports, text units, covariates)
- `ViewModels/AppStateViewModel` — Main app state (scoped per circuit)
- `ViewModels/SearchViewModel` — Search results and progress
- `ViewModels/CommunityExplorerViewModel` — Community report browsing
- `Services/SearchOrchestrator` — Parallel search execution
- `Services/KnowledgeModelService` — Data loading via DataReader
- `Services/DatasetLoader` — Dataset listing and datasource creation
- `Services/MarkdownRenderer` — Markdig pipeline wrapper

---

## Summary Statistics

| Metric | Count |
|--------|-------|
| Source projects | 24 |
| Test projects | 2 |
| Source C# files | 305+ |
| Test C# files | 100+ |
| **Total C# files** | **405+** |
| Unit tests | 274 |
| Integration tests | 5 |
| **Total tests** | **279** |
| Warnings | 0 |
| Errors | 0 |
