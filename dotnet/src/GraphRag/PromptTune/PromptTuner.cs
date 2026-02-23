// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Models;
using GraphRag.Llm;
using GraphRag.PromptTune.Generator;
using GraphRag.PromptTune.Loader;
using GraphRag.PromptTune.PromptCreator;

namespace GraphRag.PromptTune;

/// <summary>
/// Orchestrates the full prompt tuning flow: load docs, detect domain/language,
/// generate persona, identify entity types, generate examples, and create prompts.
/// </summary>
public sealed class PromptTuner
{
    /// <summary>
    /// Generates indexing prompts tuned to the provided corpus.
    /// </summary>
    /// <param name="config">The GraphRag configuration.</param>
    /// <param name="model">The LLM completion provider.</param>
    /// <param name="outputPath">The directory to write generated prompts to.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task GenerateIndexingPromptsAsync(
        GraphRagConfig config,
        ILlmCompletion model,
        string outputPath,
        CancellationToken ct)
    {
        // Step 1: Load and chunk documents.
        IReadOnlyList<string> docs = await DocumentLoader.LoadDocsInChunksAsync(
            config,
            DocSelectionType.Auto,
            PromptTuneDefaults.Limit,
            ct).ConfigureAwait(false);

        // Step 2: Detect domain.
        var domainGenerator = new DomainGenerator();
        string domain = await domainGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 3: Detect language.
        var languageDetector = new LanguageDetector();
        string language = await languageDetector.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 4: Generate persona.
        var personaGenerator = new PersonaGenerator();
        string persona = await personaGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 5: Identify entity types.
        var entityTypesGenerator = new EntityTypesGenerator();
        string entityTypes = await entityTypesGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 6: Generate entity-relationship examples.
        var entityRelationshipGenerator = new EntityRelationshipGenerator();
        string examples = await entityRelationshipGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 7: Generate community report artifacts.
        var communityRoleGenerator = new CommunityReporterRoleGenerator();
        string role = await communityRoleGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        var communityRatingGenerator = new CommunityReportRatingGenerator();
        string rating = await communityRatingGenerator.GenerateAsync(model, docs, ct).ConfigureAwait(false);

        // Step 8: Create prompts.
        string extractGraphPrompt = ExtractGraphPromptCreator.CreateExtractGraphPrompt(entityTypes, examples, language);
        string entitySummarizationPrompt = EntitySummarizationPromptCreator.CreateEntitySummarizationPrompt(persona, language);
        string communitySummarizationPrompt = CommunitySummarizationPromptCreator.CreateCommunitySummarizationPrompt(role, rating, language);

        // Step 9: Write prompts to output path.
        Directory.CreateDirectory(outputPath);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "extract_graph.txt"), extractGraphPrompt, ct).ConfigureAwait(false);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "entity_summarization.txt"), entitySummarizationPrompt, ct).ConfigureAwait(false);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "community_summarization.txt"), communitySummarizationPrompt, ct).ConfigureAwait(false);
    }
}
