// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Catalyst;
using Catalyst.Models;

using GraphRag.Common.Discovery;

using Mosaik.Core;

namespace GraphRag.Nlp.Catalyst;

/// <summary>
/// Extracts noun phrases from text using the Catalyst NLP pipeline.
/// </summary>
[StrategyImplementation("catalyst", typeof(INounPhraseExtractor))]
public sealed class CatalystNounPhraseExtractor : INounPhraseExtractor, IDisposable
{
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private Pipeline? _pipeline;

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ExtractNounPhrasesAsync(
        string text,
        CancellationToken ct = default)
    {
        var pipeline = await GetPipelineAsync(ct).ConfigureAwait(false);
        var doc = new Document(text, Language.English);
        pipeline.ProcessSingle(doc);

        var phrases = new List<string>();
        foreach (var span in doc)
        {
            foreach (var token in span.Tokens)
            {
                var pos = token.POS;
                if (pos is PartOfSpeech.NOUN or PartOfSpeech.PROPN)
                {
                    phrases.Add(token.Value);
                }
            }
        }

        return phrases;
    }

    /// <summary>
    /// Releases the resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        _initLock.Dispose();
    }

    private async Task<Pipeline> GetPipelineAsync(CancellationToken ct)
    {
        if (_pipeline is not null)
        {
            return _pipeline;
        }

        await _initLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (_pipeline is not null)
            {
                return _pipeline;
            }

            Storage.Current = new DiskStorage("catalyst-models");
            var pipeline = new Pipeline(Language.English);
            var tagger = await AveragePerceptronTagger.FromStoreAsync(
                Language.English, -1, string.Empty).ConfigureAwait(false);
            pipeline.Add(tagger);
            _pipeline = pipeline;
            return _pipeline;
        }
        finally
        {
            _initLock.Release();
        }
    }
}
