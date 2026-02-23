# Strategy Isolation Plan — 3rd-Party Dependency Encapsulation

> Identifies all 3rd-party dependencies, isolates each behind the strategy pattern in a dedicated library, and defines a runtime discovery mechanism with configurable default implementations.

---

## Table of Contents

1. [Problem Statement](#problem-statement)
2. [Current 3rd-Party Dependency Audit](#current-3rd-party-dependency-audit)
3. [Strategy Isolation Architecture](#strategy-isolation-architecture)
4. [New Library Definitions](#new-library-definitions)
5. [Runtime Strategy Discovery](#runtime-strategy-discovery)
6. [Configuration Schema](#configuration-schema)
7. [Migration Steps](#migration-steps)
8. [Before & After Dependency Graph](#before--after-dependency-graph)

---

## Problem Statement

The current solution declares 3rd-party NuGet packages (Azure SDKs, Parquet.Net, CsvHelper, Catalyst, etc.) directly in the core library `.csproj` files. This creates several issues:

- **Tight coupling**: Core libraries carry transitive dependencies on Azure, OpenAI, and data-processing SDKs even when those implementations aren't used.
- **Binary bloat**: Applications that only need file-based storage still pull in Azure.Storage.Blobs, Microsoft.Azure.Cosmos, etc.
- **Testability**: Harder to test core logic in isolation when external SDKs are compiled into the same assembly.
- **Extensibility**: Adding a new storage backend (e.g., S3, GCS) requires modifying the core library.

### Goal

Apply the **strategy pattern** to isolate every 3rd-party integration into its own dedicated library. Core projects depend only on abstractions (interfaces). Concrete implementations live in separate assemblies discovered at runtime via configuration.

---

## Current 3rd-Party Dependency Audit

### Dependencies by Category

#### 🗄️ Data Stores

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| Azure Blob Storage | `Azure.Storage.Blobs` (12.24.0) | GraphRag.Storage | `IStorage` |
| Azure Cosmos DB (Storage) | `Microsoft.Azure.Cosmos` (3.48.0) | GraphRag.Storage | `IStorage` |
| Azure Cosmos DB (Vectors) | `Microsoft.Azure.Cosmos` (3.48.0) | GraphRag.Vectors | `IVectorStore` |
| Azure AI Search | `Azure.Search.Documents` (11.6.0) | GraphRag.Vectors | `IVectorStore` |
| Parquet | `Parquet.Net` (5.3.0) | GraphRag.Storage | `ITableProvider` |

#### 🤖 LLM / AI Providers

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| Azure OpenAI | `Azure.AI.OpenAI` (2.2.0-beta.1) | GraphRag.Llm | `ILlmCompletion`, `ILlmEmbedding` |
| SharpToken (tiktoken) | `SharpToken` (2.0.3) | GraphRag.Llm | `ITokenizer` |
| Scriban (Jinja2-like) | `Scriban` (5.12.0) | GraphRag.Llm | `ITemplateEngine` |

#### 📊 Data Processing

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| CsvHelper | `CsvHelper` (33.0.1) | GraphRag.Input, GraphRag.Storage | `IInputReader`, `ITableProvider` |
| Markdig (Markdown) | `Markdig` (0.40.0) | GraphRag.Input | `IInputReader` |
| OpenXml | `DocumentFormat.OpenXml` | GraphRag.Input | `IInputReader` |

#### 🧠 NLP

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| Catalyst | `Catalyst` (1.0.16605) | GraphRag (main) | `INounPhraseExtractor` |

#### 📐 Math / Graph

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| QuikGraph | `QuikGraph` (2.5.0) | GraphRag (main) | Graph algorithms |
| MathNet.Numerics | `MathNet.Numerics` (5.0.0) | GraphRag.Vectors | Vector math |

#### 🔒 Auth

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| Azure Identity | `Azure.Identity` (1.13.2) | GraphRag.Storage, Llm, Vectors | Auth for all Azure services |

#### 🔧 Infrastructure

| Dependency | NuGet Package | Used In | Interface |
|-----------|---------------|---------|-----------|
| Polly | `Polly` | GraphRag.Llm | `IRetry` (alternative) |
| FluentValidation | `FluentValidation` | GraphRag (main) | Config validation |
| Spectre.Console | `Spectre.Console` (0.49.1) | GraphRag (main) | CLI output formatting |
| System.CommandLine | `System.CommandLine` (2.0.0-beta5) | GraphRag (main) | CLI framework |

---

## Strategy Isolation Architecture

### Design Principles

1. **Core libraries contain ONLY interfaces and pure-BCL implementations** (no 3rd-party NuGet refs).
2. **Each 3rd-party integration lives in its own assembly** named `GraphRag.{Area}.{Provider}`.
3. **Runtime discovery** loads strategy assemblies via `System.Reflection` and `Microsoft.Extensions.DependencyInjection`.
4. **Configuration** maps strategy type strings to assembly-qualified type names.
5. **Fallback defaults** ensure the system works out-of-the-box with pure-.NET implementations.

### Interface ↔ Strategy Library Mapping

```
┌──────────────────────┐     ┌─────────────────────────────────┐
│   Core Library        │     │   Strategy Libraries             │
│   (interfaces only)   │     │   (one per 3rd-party dep)        │
├──────────────────────┤     ├─────────────────────────────────┤
│ GraphRag.Storage      │────▶│ GraphRag.Storage.AzureBlob      │
│   IStorage            │     │ GraphRag.Storage.AzureCosmos     │
│   ITableProvider      │────▶│ GraphRag.Storage.Parquet         │
│                       │     │ GraphRag.Storage.Csv             │
├──────────────────────┤     ├─────────────────────────────────┤
│ GraphRag.Vectors      │────▶│ GraphRag.Vectors.AzureAiSearch   │
│   IVectorStore        │     │ GraphRag.Vectors.AzureCosmos     │
│                       │     │ GraphRag.Vectors.LanceDb         │
├──────────────────────┤     ├─────────────────────────────────┤
│ GraphRag.Llm          │────▶│ GraphRag.Llm.AzureOpenAi        │
│   ILlmCompletion      │     │ GraphRag.Llm.SharpToken          │
│   ILlmEmbedding       │     │ GraphRag.Llm.Scriban             │
│   ITokenizer          │     │                                   │
│   ITemplateEngine     │     │                                   │
├──────────────────────┤     ├─────────────────────────────────┤
│ GraphRag.Input        │────▶│ GraphRag.Input.CsvHelper         │
│   IInputReader        │     │ GraphRag.Input.Markdig           │
│                       │     │ GraphRag.Input.OpenXml           │
├──────────────────────┤     ├─────────────────────────────────┤
│ GraphRag (main)       │────▶│ GraphRag.Nlp.Catalyst            │
│   INounPhraseExtractor│     │ GraphRag.Graph.QuikGraph          │
└──────────────────────┘     └─────────────────────────────────┘
```

---

## New Library Definitions

### 4.1 Storage Strategy Libraries

#### `GraphRag.Storage.AzureBlob`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Azure.Storage.Blobs`, `Azure.Identity` |
| **Implements** | `IStorage` |
| **Class** | `AzureBlobStorage` |
| **Purpose** | Read/write blobs in Azure Blob Storage containers. Supports SAS, connection string, and managed identity auth. |

#### `GraphRag.Storage.AzureCosmos`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Microsoft.Azure.Cosmos`, `Azure.Identity`, `Newtonsoft.Json` |
| **Implements** | `IStorage` |
| **Class** | `AzureCosmosStorage` |
| **Purpose** | Document-oriented storage using Cosmos DB containers with partition key support. |

#### `GraphRag.Storage.Parquet`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Parquet.Net` |
| **Implements** | `ITableProvider`, `ITable` |
| **Classes** | `ParquetTableProvider`, `ParquetTable` |
| **Purpose** | Read/write columnar Parquet files for high-performance tabular data. |

#### `GraphRag.Storage.Csv`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `CsvHelper` |
| **Implements** | `ITableProvider`, `ITable` |
| **Classes** | `CsvTableProvider`, `CsvTable` |
| **Purpose** | Read/write CSV files with header mapping and type coercion. |

---

### 4.2 Vector Store Strategy Libraries

#### `GraphRag.Vectors.AzureAiSearch`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Azure.Search.Documents`, `Azure.Identity` |
| **Implements** | `IVectorStore` |
| **Class** | `AzureAiSearchVectorStore` |
| **Purpose** | Vector similarity search via Azure AI Search with OData filter compilation. |

#### `GraphRag.Vectors.AzureCosmos`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Microsoft.Azure.Cosmos`, `Azure.Identity`, `Newtonsoft.Json` |
| **Implements** | `IVectorStore` |
| **Class** | `CosmosDbVectorStore` |
| **Purpose** | Vector search using Cosmos DB's built-in vector indexing with SQL filter compilation. |

#### `GraphRag.Vectors.LanceDb`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | LanceDB .NET client (TBD) |
| **Implements** | `IVectorStore` |
| **Class** | `LanceDbVectorStore` |
| **Purpose** | Embedded vector search via LanceDB with local file URI support. |

---

### 4.3 LLM Strategy Libraries

#### `GraphRag.Llm.AzureOpenAi`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Azure.AI.OpenAI`, `Azure.Identity` |
| **Implements** | `ILlmCompletion`, `ILlmEmbedding` |
| **Classes** | `AzureOpenAiCompletion`, `AzureOpenAiEmbedding` |
| **Purpose** | Chat completions and text embeddings via Azure OpenAI or OpenAI API. Supports streaming, tool calls, structured output. |

#### `GraphRag.Llm.SharpToken`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `SharpToken` |
| **Implements** | `ITokenizer` |
| **Class** | `SharpTokenTokenizer` |
| **Purpose** | Accurate BPE token counting using tiktoken-compatible encoding models (cl100k_base, o200k_base). |

#### `GraphRag.Llm.Scriban`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Scriban` |
| **Implements** | `ITemplateEngine` |
| **Class** | `ScribanTemplateEngine` |
| **Purpose** | Full Jinja2-compatible template rendering with filters, loops, conditionals — replacing the basic `{{variable}}` engine. |

---

### 4.4 Input Strategy Libraries

#### `GraphRag.Input.CsvHelper`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `CsvHelper` |
| **Implements** | `IInputReader` |
| **Class** | `CsvHelperFileReader` |
| **Purpose** | Robust CSV parsing with header mapping, culture handling, and missing field tolerance. |

#### `GraphRag.Input.Markdig`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Markdig` |
| **Implements** | `IInputReader` |
| **Class** | `MarkdownFileReader` |
| **Purpose** | Convert Markdown documents to plain text for indexing. |

#### `GraphRag.Input.OpenXml`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `DocumentFormat.OpenXml` |
| **Implements** | `IInputReader` |
| **Class** | `OpenXmlFileReader` |
| **Purpose** | Extract text from .docx, .xlsx, .pptx documents. |

---

### 4.5 NLP / Graph Strategy Libraries

#### `GraphRag.Nlp.Catalyst`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `Catalyst`, `Catalyst.Models.English` |
| **Implements** | `INounPhraseExtractor` |
| **Class** | `CatalystNounPhraseExtractor` |
| **Purpose** | Production NLP-based noun phrase extraction with POS tagging and named entity recognition. |

#### `GraphRag.Graph.QuikGraph`

| Attribute | Value |
|-----------|-------|
| **NuGet Deps** | `QuikGraph` |
| **Implements** | Graph algorithm interfaces |
| **Class** | `QuikGraphAlgorithms` |
| **Purpose** | Leiden/Louvain community detection, connected components, centrality computation. |

---

## Runtime Strategy Discovery

### 5.1 Discovery Mechanism

A new `GraphRag.Common.Discovery` namespace provides assembly-scanning strategy resolution:

```csharp
namespace GraphRag.Common.Discovery;

/// <summary>
/// Discovers and registers strategy implementations at runtime
/// from configured assemblies.
/// </summary>
public sealed class StrategyDiscovery
{
    /// <summary>
    /// Scan an assembly for types implementing a given interface
    /// and register them in the service collection.
    /// </summary>
    public void DiscoverStrategies<TInterface>(
        IServiceCollection services,
        Assembly assembly);

    /// <summary>
    /// Load an assembly by name, scan it, and register all
    /// strategy implementations found.
    /// </summary>
    public void DiscoverStrategies<TInterface>(
        IServiceCollection services,
        string assemblyName);

    /// <summary>
    /// Register all strategies defined in a StrategyConfiguration.
    /// </summary>
    public void RegisterFromConfiguration(
        IServiceCollection services,
        StrategyConfiguration config);
}
```

### 5.2 Strategy Attribute

Each strategy implementation is decorated with a discoverable attribute:

```csharp
namespace GraphRag.Common.Discovery;

/// <summary>
/// Marks a class as a strategy implementation discoverable at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class StrategyImplementationAttribute : Attribute
{
    /// <summary>Gets the strategy type key (e.g., "blob", "cosmosdb").</summary>
    public string StrategyKey { get; }

    /// <summary>Gets the interface this class implements.</summary>
    public Type InterfaceType { get; }

    public StrategyImplementationAttribute(
        string strategyKey, Type interfaceType);
}
```

### 5.3 Usage Example

```csharp
// In GraphRag.Storage.AzureBlob:
[StrategyImplementation("blob", typeof(IStorage))]
public sealed class AzureBlobStorage : IStorage
{
    // ...
}

// At startup:
var discovery = new StrategyDiscovery();
discovery.DiscoverStrategies<IStorage>(services, "GraphRag.Storage.AzureBlob");
```

### 5.4 Auto-Discovery via Assembly Convention

Assemblies following the naming convention `GraphRag.*.dll` in the application directory are automatically scanned at startup. This allows "drop-in" deployment of strategy libraries without code changes.

```csharp
// In Program.cs or host builder:
var strategyAssemblies = Directory.GetFiles(
    AppContext.BaseDirectory, "GraphRag.*.dll");

foreach (var assemblyPath in strategyAssemblies)
{
    var assembly = Assembly.LoadFrom(assemblyPath);
    discovery.DiscoverStrategies<IStorage>(services, assembly);
    discovery.DiscoverStrategies<IVectorStore>(services, assembly);
    discovery.DiscoverStrategies<ILlmCompletion>(services, assembly);
    // ... etc.
}
```

---

## Configuration Schema

### 6.1 Strategy Configuration Section

Add a new `strategies` section to `settings.yaml`:

```yaml
strategies:
  # Default implementations used when no specific type is configured.
  # Maps interface category → strategy key → assembly.
  defaults:
    storage: file                  # Built-in (no extra assembly needed)
    cache: json                    # Built-in
    vector_store: lancedb          # Requires GraphRag.Vectors.LanceDb.dll
    completion: azure_openai       # Requires GraphRag.Llm.AzureOpenAi.dll
    embedding: azure_openai        # Requires GraphRag.Llm.AzureOpenAi.dll
    tokenizer: sharptoken          # Requires GraphRag.Llm.SharpToken.dll
    template_engine: scriban       # Requires GraphRag.Llm.Scriban.dll
    table_provider: parquet        # Requires GraphRag.Storage.Parquet.dll
    input_reader: text             # Built-in
    noun_phrase_extractor: regex   # Built-in

  # Explicit assembly mappings (override auto-discovery).
  assemblies:
    - GraphRag.Storage.AzureBlob
    - GraphRag.Storage.AzureCosmos
    - GraphRag.Storage.Parquet
    - GraphRag.Storage.Csv
    - GraphRag.Vectors.AzureAiSearch
    - GraphRag.Vectors.AzureCosmos
    - GraphRag.Vectors.LanceDb
    - GraphRag.Llm.AzureOpenAi
    - GraphRag.Llm.SharpToken
    - GraphRag.Llm.Scriban
    - GraphRag.Input.CsvHelper
    - GraphRag.Nlp.Catalyst
    - GraphRag.Graph.QuikGraph

  # Per-strategy-key configuration overrides.
  overrides:
    blob:
      assembly: GraphRag.Storage.AzureBlob
      type: GraphRag.Storage.AzureBlob.AzureBlobStorage
    azure_openai:
      assembly: GraphRag.Llm.AzureOpenAi
      type: GraphRag.Llm.AzureOpenAi.AzureOpenAiCompletion
```

### 6.2 C# Configuration Model

```csharp
namespace GraphRag.Common.Discovery;

public sealed record StrategyConfiguration
{
    /// <summary>Gets the default strategy key for each interface category.</summary>
    public Dictionary<string, string> Defaults { get; init; } = new();

    /// <summary>Gets the list of assemblies to scan for strategies.</summary>
    public IReadOnlyList<string> Assemblies { get; init; } = [];

    /// <summary>Gets per-key overrides with explicit assembly and type.</summary>
    public Dictionary<string, StrategyOverride> Overrides { get; init; } = new();
}

public sealed record StrategyOverride
{
    /// <summary>Gets the assembly name containing the implementation.</summary>
    public string? Assembly { get; init; }

    /// <summary>Gets the fully-qualified type name of the implementation.</summary>
    public string? Type { get; init; }
}
```

### 6.3 Factory Integration

Each existing factory (`StorageFactory`, `VectorStoreFactory`, etc.) gains a `RegisterFromDiscovery` method:

```csharp
public void RegisterFromDiscovery(StrategyDiscovery discovery, StrategyConfiguration config)
{
    // Auto-register all discovered implementations for this factory's interface.
    // The factory's existing Register(key, factory_func) API is used under the hood.
}
```

---

## Migration Steps

### Step 1: Clean Core Libraries

Remove all 3rd-party NuGet `<PackageReference>` entries from core `.csproj` files. Core projects should only reference:

| Project | Allowed NuGet Packages |
|---------|----------------------|
| GraphRag.Common | `YamlDotNet`, `Microsoft.Extensions.DependencyInjection.Abstractions` |
| GraphRag.Storage | **None** (pure .NET + project refs only) |
| GraphRag.Cache | **None** |
| GraphRag.Chunking | **None** |
| GraphRag.Input | **None** (remove CsvHelper — move to strategy lib) |
| GraphRag.Llm | `Microsoft.Extensions.Logging.Abstractions` only |
| GraphRag.Vectors | `MathNet.Numerics` only (pure math, no external service) |
| GraphRag (main) | `System.CommandLine`, `Spectre.Console`, `FluentValidation`, `Microsoft.Extensions.*` |

### Step 2: Create Strategy Libraries

For each library in [Section 4](#new-library-definitions):

1. `dotnet new classlib -n <LibraryName>` in `dotnet/src/`
2. Add `<ProjectReference>` to the core interface project
3. Add `<PackageReference>` to the 3rd-party NuGet package
4. Move existing stub/implementation code from core to strategy library
5. Add `[StrategyImplementation]` attribute to each class
6. Add `.csproj` to `GraphRag.slnx`

### Step 3: Implement Discovery Infrastructure

1. Create `GraphRag.Common.Discovery` namespace with:
   - `StrategyImplementationAttribute`
   - `StrategyDiscovery`
   - `StrategyConfiguration` / `StrategyOverride`
2. Update `ServiceFactory<T>` to support `RegisterFromDiscovery`
3. Update `ConfigLoader` to parse the `strategies:` section

### Step 4: Wire Up at Startup

Update `Program.cs`:

```csharp
var config = ConfigLoader.Load<GraphRagConfig>(rootDir);
var strategyConfig = config.Strategies ?? StrategyConfiguration.Default;

var discovery = new StrategyDiscovery();
discovery.RegisterFromConfiguration(services, strategyConfig);

// Factories now resolve implementations from DI container
var storageFactory = new StorageFactory(services);
var vectorFactory = new VectorStoreFactory(services);
// ...
```

### Step 5: Update Tests

1. Unit tests continue using pure-.NET implementations (no change needed)
2. Integration tests for strategy libraries go in dedicated test projects:
   - `GraphRag.Tests.Integration.Azure` (requires Azure credentials)
   - `GraphRag.Tests.Integration.LanceDb` (local file)
3. Add mock/stub strategy assemblies for CI testing without cloud dependencies

### Step 6: Update Packaging

Each strategy library becomes an independent NuGet package:

```
GraphRag.Storage.AzureBlob      → nuget.org
GraphRag.Storage.AzureCosmos     → nuget.org
GraphRag.Storage.Parquet         → nuget.org
GraphRag.Vectors.AzureAiSearch   → nuget.org
GraphRag.Llm.AzureOpenAi        → nuget.org
...
```

Users install only the strategy packages they need:
```bash
dotnet add package GraphRag
dotnet add package GraphRag.Storage.AzureBlob      # only if using Azure Blob
dotnet add package GraphRag.Llm.AzureOpenAi         # only if using Azure OpenAI
```

---

## Before & After Dependency Graph

### BEFORE (current)

```
GraphRag.Storage.csproj
  ├── Azure.Storage.Blobs        ← always pulled in
  ├── Microsoft.Azure.Cosmos      ← always pulled in
  ├── Newtonsoft.Json              ← always pulled in
  ├── Parquet.Net                  ← always pulled in
  └── CsvHelper                   ← always pulled in
```

### AFTER (isolated)

```
GraphRag.Storage.csproj
  └── (no NuGet refs — pure .NET)

GraphRag.Storage.AzureBlob.csproj        ← optional
  ├── GraphRag.Storage (project ref)
  ├── Azure.Storage.Blobs
  └── Azure.Identity

GraphRag.Storage.Parquet.csproj          ← optional
  ├── GraphRag.Storage (project ref)
  └── Parquet.Net
```

### Reduced Deployment Size Example

| Scenario | Before | After |
|----------|--------|-------|
| File storage only | ~45 MB (all Azure + Parquet DLLs) | ~3 MB |
| Azure Blob + Parquet | ~45 MB | ~25 MB |
| Full (all providers) | ~45 MB | ~45 MB |

---

## Summary of New Projects to Create

| # | Project Name | Implements | Key NuGet Dep |
|---|-------------|-----------|---------------|
| 1 | `GraphRag.Storage.AzureBlob` | `IStorage` | Azure.Storage.Blobs |
| 2 | `GraphRag.Storage.AzureCosmos` | `IStorage` | Microsoft.Azure.Cosmos |
| 3 | `GraphRag.Storage.Parquet` | `ITableProvider` | Parquet.Net |
| 4 | `GraphRag.Storage.Csv` | `ITableProvider` | CsvHelper |
| 5 | `GraphRag.Vectors.AzureAiSearch` | `IVectorStore` | Azure.Search.Documents |
| 6 | `GraphRag.Vectors.AzureCosmos` | `IVectorStore` | Microsoft.Azure.Cosmos |
| 7 | `GraphRag.Vectors.LanceDb` | `IVectorStore` | LanceDB client |
| 8 | `GraphRag.Llm.AzureOpenAi` | `ILlmCompletion`, `ILlmEmbedding` | Azure.AI.OpenAI |
| 9 | `GraphRag.Llm.SharpToken` | `ITokenizer` | SharpToken |
| 10 | `GraphRag.Llm.Scriban` | `ITemplateEngine` | Scriban |
| 11 | `GraphRag.Input.CsvHelper` | `IInputReader` | CsvHelper |
| 12 | `GraphRag.Input.Markdig` | `IInputReader` | Markdig |
| 13 | `GraphRag.Input.OpenXml` | `IInputReader` | DocumentFormat.OpenXml |
| 14 | `GraphRag.Nlp.Catalyst` | `INounPhraseExtractor` | Catalyst |
| 15 | `GraphRag.Graph.QuikGraph` | Graph algorithms | QuikGraph |

**Total: 15 new strategy libraries + 1 discovery infrastructure update to GraphRag.Common.**
