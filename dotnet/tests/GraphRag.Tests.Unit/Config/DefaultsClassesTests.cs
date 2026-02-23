// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Defaults;

namespace GraphRag.Tests.Unit.Config;

public class DefaultsClassesTests
{
    [Fact]
    public void DefaultValues_AuthTypes_AreCorrect()
    {
        DefaultValues.DefaultCompletionModelAuthType.Should().Be("api_key");
        DefaultValues.DefaultEmbeddingModelAuthType.Should().Be("api_key");
    }

    [Fact]
    public void DefaultValues_CognitiveServicesAudience_IsCorrect()
    {
        DefaultValues.CognitiveServicesAudience.Should().Be("https://cognitiveservices.azure.com/.default");
    }

    [Fact]
    public void BasicSearchDefaults_ValuesAreCorrect()
    {
        BasicSearchDefaults.K.Should().Be(20);
        BasicSearchDefaults.MaxContextTokens.Should().Be(8000);
    }

    [Fact]
    public void ChunkingDefaults_ValuesAreCorrect()
    {
        ChunkingDefaults.Size.Should().Be(1200);
        ChunkingDefaults.Overlap.Should().Be(100);
        ChunkingDefaults.EncodingModel.Should().Be("o200k_base");
        ChunkingDefaults.GroupByColumns.Should().Contain("id");
    }

    [Fact]
    public void ClusterGraphDefaults_ValuesAreCorrect()
    {
        ClusterGraphDefaults.MaxClusterSize.Should().Be(10);
        ClusterGraphDefaults.UseLcc.Should().BeTrue();
    }

    [Fact]
    public void GlobalSearchDefaults_ValuesAreCorrect()
    {
        GlobalSearchDefaults.MaxContextTokens.Should().Be(8000);
        GlobalSearchDefaults.DataMaxTokens.Should().Be(12000);
        GlobalSearchDefaults.MapMaxLength.Should().Be(1000);
        GlobalSearchDefaults.ReduceMaxLength.Should().Be(2000);
    }

    [Fact]
    public void LocalSearchDefaults_ValuesAreCorrect()
    {
        LocalSearchDefaults.TextUnitProp.Should().Be(0.5);
        LocalSearchDefaults.CommunityProp.Should().Be(0.1);
        LocalSearchDefaults.TopKEntities.Should().Be(10);
        LocalSearchDefaults.MaxContextTokens.Should().Be(12000);
    }

    [Fact]
    public void StorageDefaults_ValuesAreCorrect()
    {
        StorageDefaults.Type.Should().Be("file");
        StorageDefaults.Encoding.Should().Be("utf-8");
    }

    [Fact]
    public void PruneGraphDefaults_ValuesAreCorrect()
    {
        PruneGraphDefaults.MinNodeFreq.Should().Be(0);
        PruneGraphDefaults.MinNodeDegree.Should().Be(0);
        PruneGraphDefaults.RemoveEgoNodes.Should().BeFalse();
        PruneGraphDefaults.LccOnly.Should().BeFalse();
    }

    [Fact]
    public void SnapshotsDefaults_AllFalse()
    {
        SnapshotsDefaults.Embeddings.Should().BeFalse();
        SnapshotsDefaults.GraphMl.Should().BeFalse();
        SnapshotsDefaults.RawGraph.Should().BeFalse();
    }

    [Fact]
    public void VectorStoreDefaults_ValuesAreCorrect()
    {
        VectorStoreDefaults.Type.Should().Be("lancedb");
        VectorStoreDefaults.DbUri.Should().Be("output/lancedb");
    }

    [Fact]
    public void GraphRagConfigDefaults_ValuesAreCorrect()
    {
        GraphRagConfigDefaults.ConcurrentRequests.Should().Be(25);
        GraphRagConfigDefaults.AsyncMode.Should().Be("threaded");
    }

    [Fact]
    public void DriftSearchDefaults_KeyValuesAreCorrect()
    {
        DriftSearchDefaults.DataMaxTokens.Should().Be(12000);
        DriftSearchDefaults.Concurrency.Should().Be(4);
        DriftSearchDefaults.DriftKFollowups.Should().Be(3);
        DriftSearchDefaults.NDepth.Should().Be(3);
    }
}
