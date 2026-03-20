// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Defaults;
using GraphRag.Config.Errors;
using GraphRag.Config.Models;
using GraphRag.Llm.Config;
using GraphRag.Llm.Types;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Config;

public class GraphRagConfigMethodTests
{
    [Fact]
    public void GetCompletionModelConfig_ReturnsConfigForKey()
    {
        var modelConfig = new ModelConfig { Model = "gpt-4.1" };
        var config = new GraphRagConfig
        {
            CompletionModels = new Dictionary<string, ModelConfig>
            {
                ["my_model"] = modelConfig,
            },
        };

        var result = config.GetCompletionModelConfig("my_model");

        result.Should().BeSameAs(modelConfig);
    }

    [Fact]
    public void GetCompletionModelConfig_UsesDefaultKey_WhenNull()
    {
        var modelConfig = new ModelConfig { Model = "gpt-4.1" };
        var config = new GraphRagConfig
        {
            CompletionModels = new Dictionary<string, ModelConfig>
            {
                [DefaultValues.DefaultCompletionModelId] = modelConfig,
            },
        };

        var result = config.GetCompletionModelConfig();

        result.Should().BeSameAs(modelConfig);
    }

    [Fact]
    public void GetCompletionModelConfig_ThrowsForMissingKey()
    {
        var config = new GraphRagConfig();

        var act = () => config.GetCompletionModelConfig("nonexistent");

        act.Should().Throw<KeyNotFoundException>()
           .WithMessage("*nonexistent*");
    }

    [Fact]
    public void GetEmbeddingModelConfig_ReturnsConfigForKey()
    {
        var modelConfig = new ModelConfig { Model = "text-embedding-3-large" };
        var config = new GraphRagConfig
        {
            EmbeddingModels = new Dictionary<string, ModelConfig>
            {
                ["emb1"] = modelConfig,
            },
        };

        var result = config.GetEmbeddingModelConfig("emb1");

        result.Should().BeSameAs(modelConfig);
    }

    [Fact]
    public void GetEmbeddingModelConfig_UsesDefaultKey_WhenNull()
    {
        var modelConfig = new ModelConfig { Model = "text-embedding-3-large" };
        var config = new GraphRagConfig
        {
            EmbeddingModels = new Dictionary<string, ModelConfig>
            {
                [DefaultValues.DefaultEmbeddingModelId] = modelConfig,
            },
        };

        var result = config.GetEmbeddingModelConfig();

        result.Should().BeSameAs(modelConfig);
    }

    [Fact]
    public void GetEmbeddingModelConfig_ThrowsForMissingKey()
    {
        var config = new GraphRagConfig();

        var act = () => config.GetEmbeddingModelConfig("missing");

        act.Should().Throw<KeyNotFoundException>()
           .WithMessage("*missing*");
    }

    [Fact]
    public void TableProvider_DefaultIsNull()
    {
        var config = new GraphRagConfig();

        config.TableProvider.Should().BeNull();
    }

    [Fact]
    public void Workflows_DefaultIsNull()
    {
        var config = new GraphRagConfig();

        config.Workflows.Should().BeNull();
    }

    [Fact]
    public void TableProvider_CanBeSet()
    {
        var config = new GraphRagConfig { TableProvider = "parquet" };

        config.TableProvider.Should().Be("parquet");
    }

    [Fact]
    public void Workflows_CanBeSet()
    {
        var workflows = new[] { "extract_graph", "finalize_graph" };
        var config = new GraphRagConfig { Workflows = workflows };

        config.Workflows.Should().BeEquivalentTo(workflows);
    }

    [Fact]
    public void SyncVectorStoreDimensions_UpdatesVectorStoreAndSchema_ForConfiguredEmbeddingModel()
    {
        var config = new GraphRagConfig
        {
            EmbedText = new EmbedTextConfig { EmbeddingModelId = "embed-model" },
            VectorStore = new VectorStoreConfig
            {
                Type = "azure_ai_search",
                VectorSize = 3072,
                IndexSchema = new IndexSchema { IndexName = "entities", VectorSize = 3072 },
            },
        };
        var response = new LlmEmbeddingResponse([[1.0f, 2.0f, 3.0f]]);

        var result = config.SyncVectorStoreDimensions("embed-model", response);

        result.Should().NotBeSameAs(config);
        result.VectorStore.VectorSize.Should().Be(3);
        result.VectorStore.IndexSchema.Should().NotBeNull();
        result.VectorStore.IndexSchema!.VectorSize.Should().Be(3);
        config.VectorStore.VectorSize.Should().Be(3072);
        config.VectorStore.IndexSchema!.VectorSize.Should().Be(3072);
    }

    [Fact]
    public void SyncVectorStoreDimensions_ReturnsSameConfig_WhenEmbeddingModelDoesNotMatch()
    {
        var config = new GraphRagConfig
        {
            EmbedText = new EmbedTextConfig { EmbeddingModelId = "embed-model" },
        };
        var response = new LlmEmbeddingResponse([[1.0f, 2.0f, 3.0f]]);

        var result = config.SyncVectorStoreDimensions("different-model", response);

        result.Should().BeSameAs(config);
    }

    [Fact]
    public void SyncVectorStoreDimensions_ReturnsSameConfig_WhenResponseIsEmpty()
    {
        var config = new GraphRagConfig
        {
            EmbedText = new EmbedTextConfig { EmbeddingModelId = "embed-model" },
        };
        var response = new LlmEmbeddingResponse([]);

        var result = config.SyncVectorStoreDimensions("embed-model", response);

        result.Should().BeSameAs(config);
    }
}
