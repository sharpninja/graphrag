# GraphRAG

👉 [Microsoft Research Blog Post](https://www.microsoft.com/en-us/research/blog/graphrag-unlocking-llm-discovery-on-narrative-private-data/)<br/>
👉 [Read the docs](https://microsoft.github.io/graphrag)<br/>
👉 [GraphRAG Arxiv](https://arxiv.org/pdf/2404.16130)

<div align="left">
  <a href="https://pypi.org/project/graphrag/">
    <img alt="PyPI - Version" src="https://img.shields.io/pypi/v/graphrag">
  </a>
  <a href="https://pypi.org/project/graphrag/">
    <img alt="PyPI - Downloads" src="https://img.shields.io/pypi/dm/graphrag">
  </a>
  <a href="https://github.com/microsoft/graphrag/issues">
    <img alt="GitHub Issues" src="https://img.shields.io/github/issues/microsoft/graphrag">
  </a>
  <a href="https://github.com/microsoft/graphrag/discussions">
    <img alt="GitHub Discussions" src="https://img.shields.io/github/discussions/microsoft/graphrag">
  </a>
</div>

## Overview

The GraphRAG project is a data pipeline and transformation suite that is designed to extract meaningful, structured data from unstructured text using the power of LLMs. GraphRAG is available in both **Python** and **.NET 10 C#**, with identical functionality and configuration format across both implementations.

To learn more about GraphRAG and how it can be used to enhance your LLM's ability to reason about your private data, please visit the <a href="https://www.microsoft.com/en-us/research/blog/graphrag-unlocking-llm-discovery-on-narrative-private-data/" target="_blank">Microsoft Research Blog Post.</a>

## Implementations

| | Python | .NET |
|---|--------|------|
| **Runtime** | Python 3.10–3.12 | .NET 10 (C#) |
| **Packages** | 8 packages on [PyPI](https://pypi.org/project/graphrag/) | 8 core libraries + 15 strategy plugin assemblies |
| **Architecture** | Factory pattern with ABC interfaces | Strategy pattern with runtime assembly discovery |
| **Config Format** | `settings.yaml` | `settings.yaml` (shared format) |
| **CLI** | `graphrag` (via pip) | `dotnet run --project src/GraphRag` |
| **Tests** | pytest (unit, integration, smoke) | xUnit (200 tests — unit + integration) |
| **Getting Started** | [Python Quickstart](https://microsoft.github.io/graphrag/get_started/) | [.NET Getting Started](dotnet/docs/getting-started.md) |
| **Search UI** | `unified-search-app/` (Streamlit) | `GraphRag.SearchApp` (Blazor) — [Getting Started](dotnet/docs/search-app-getting-started.md) |

## Quickstart

### Python

To get started with the Python implementation, we recommend trying the [command line quickstart](https://microsoft.github.io/graphrag/get_started/).

```bash
pip install graphrag
graphrag init
# Add your documents to ./input/
graphrag index
graphrag query "What are the top themes in this story?"
```

### .NET

For the .NET implementation, see the full [.NET Getting Started Guide](dotnet/docs/getting-started.md).

```bash
git clone https://github.com/microsoft/graphrag.git
cd graphrag/dotnet
dotnet build
dotnet run --project src/GraphRag -- init --root ./my-project
# Add your documents to ./my-project/input/
dotnet run --project src/GraphRag -- index --root ./my-project
dotnet run --project src/GraphRag -- query --root ./my-project --method local --query "What are the top themes?"
```

## Repository Structure

```
graphrag/
├── packages/               ← Python monorepo (8 packages)
│   ├── graphrag/           ← Main Python package (CLI, indexing, query)
│   ├── graphrag-common/    ← Shared utilities
│   ├── graphrag-storage/   ← Storage backends
│   ├── graphrag-cache/     ← Caching layer
│   ├── graphrag-chunking/  ← Text chunking
│   ├── graphrag-input/     ← Document ingestion
│   ├── graphrag-llm/       ← LLM abstraction
│   └── graphrag-vectors/   ← Vector stores
├── dotnet/                 ← .NET 10 implementation
│   ├── src/                ← 8 core libraries + 15 strategy plugins + SearchApp
│   ├── tests/              ← Unit + integration tests
│   └── docs/               ← .NET-specific documentation
├── docs/                   ← MkDocs documentation site (Python-focused)
├── tests/                  ← Python test suite
└── scripts/                ← Build & CI scripts
```

## Repository Guidance

This repository presents a methodology for using knowledge graph memory structures to enhance LLM outputs. Please note that the provided code serves as a demonstration and is not an officially supported Microsoft offering.

⚠️ *Warning: GraphRAG indexing can be an expensive operation, please read all of the documentation to understand the process and costs involved, and start small.*

## Diving Deeper

- To learn about our contribution guidelines, see [CONTRIBUTING.md](./CONTRIBUTING.md)
- To start developing the Python implementation, see [DEVELOPING.md](./DEVELOPING.md)
- For .NET development, see the [.NET Getting Started Guide](dotnet/docs/getting-started.md) and [.NET Project Overview](dotnet/docs/project-overview.md)
- For the .NET Search App, see the [Search App Getting Started Guide](dotnet/docs/search-app-getting-started.md)
- Join the conversation and provide feedback in the [GitHub Discussions tab!](https://github.com/microsoft/graphrag/discussions)

## Prompt Tuning

Using _GraphRAG_ with your data out of the box may not yield the best possible results.
We strongly recommend to fine-tune your prompts following the [Prompt Tuning Guide](https://microsoft.github.io/graphrag/prompt_tuning/overview/) in our documentation.

## Versioning

Please see the [breaking changes](./breaking-changes.md) document for notes on our approach to versioning the project.

**Python**: *Always run `graphrag init --root [path] --force` between minor version bumps to ensure you have the latest config format. Run the provided migration notebook between major version bumps if you want to avoid re-indexing prior datasets. Note that this will overwrite your configuration and prompts, so backup if necessary.*

**.NET**: The .NET implementation shares the same `settings.yaml` configuration format. See [dotnet/docs/getting-started.md](dotnet/docs/getting-started.md) for version-specific details.

## Responsible AI FAQ

See [RAI_TRANSPARENCY.md](./RAI_TRANSPARENCY.md)

- [What is GraphRAG?](./RAI_TRANSPARENCY.md#what-is-graphrag)
- [What can GraphRAG do?](./RAI_TRANSPARENCY.md#what-can-graphrag-do)
- [What are GraphRAG’s intended use(s)?](./RAI_TRANSPARENCY.md#what-are-graphrags-intended-uses)
- [How was GraphRAG evaluated? What metrics are used to measure performance?](./RAI_TRANSPARENCY.md#how-was-graphrag-evaluated-what-metrics-are-used-to-measure-performance)
- [What are the limitations of GraphRAG? How can users minimize the impact of GraphRAG’s limitations when using the system?](./RAI_TRANSPARENCY.md#what-are-the-limitations-of-graphrag-how-can-users-minimize-the-impact-of-graphrags-limitations-when-using-the-system)
- [What operational factors and settings allow for effective and responsible use of GraphRAG?](./RAI_TRANSPARENCY.md#what-operational-factors-and-settings-allow-for-effective-and-responsible-use-of-graphrag)

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.

## Privacy

[Microsoft Privacy Statement](https://privacy.microsoft.com/en-us/privacystatement)
