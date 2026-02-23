// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;
using GraphRag.SearchApp.Models;
using Microsoft.Extensions.Logging;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Loads the knowledge model from a datasource.
/// </summary>
public class KnowledgeModelService
{
    private readonly ILogger<KnowledgeModelService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeModelService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public KnowledgeModelService(ILogger<KnowledgeModelService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Loads all knowledge model data from the given datasource.
    /// </summary>
    /// <param name="datasource">The datasource to load data from.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The loaded knowledge model.</returns>
    public async Task<KnowledgeModel> LoadModelAsync(
        IDatasource datasource,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasource);

        _logger.LogInformation("Loading knowledge model data...");

        var entitiesTask = LoadTableAsync(datasource, "output/entities", Entity.FromDictionary, cancellationToken);
        var relationshipsTask = LoadTableAsync(datasource, "output/relationships", Relationship.FromDictionary, cancellationToken);
        var communitiesTask = LoadTableAsync(datasource, "output/communities", Community.FromDictionary, cancellationToken);
        var reportsTask = LoadTableAsync(datasource, "output/community_reports", CommunityReport.FromDictionary, cancellationToken);
        var textUnitsTask = LoadTableAsync(datasource, "output/text_units", TextUnit.FromDictionary, cancellationToken);
        var covariatesTask = LoadTableSafeAsync(datasource, "output/covariates", Covariate.FromDictionary, cancellationToken);

        await Task.WhenAll(entitiesTask, relationshipsTask, communitiesTask, reportsTask, textUnitsTask, covariatesTask).ConfigureAwait(false);

        var model = new KnowledgeModel
        {
            Entities = await entitiesTask.ConfigureAwait(false),
            Relationships = await relationshipsTask.ConfigureAwait(false),
            Communities = await communitiesTask.ConfigureAwait(false),
            CommunityReports = await reportsTask.ConfigureAwait(false),
            TextUnits = await textUnitsTask.ConfigureAwait(false),
            Covariates = await covariatesTask.ConfigureAwait(false),
        };

        _logger.LogInformation(
            "Knowledge model loaded: {Entities} entities, {Rels} relationships, {Communities} communities, {Reports} reports.",
            model.Entities.Count,
            model.Relationships.Count,
            model.Communities.Count,
            model.CommunityReports.Count);

        return model;
    }

    private static async Task<IReadOnlyList<T>> LoadTableAsync<T>(
        IDatasource datasource,
        string tableName,
        Func<Dictionary<string, object?>, T> mapper,
        CancellationToken cancellationToken)
    {
        var rows = await datasource.ReadTableAsync(tableName, cancellationToken).ConfigureAwait(false);
        return rows.Select(mapper).ToList();
    }

    private async Task<IReadOnlyList<T>> LoadTableSafeAsync<T>(
        IDatasource datasource,
        string tableName,
        Func<Dictionary<string, object?>, T> mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await datasource.HasTableAsync(tableName, cancellationToken).ConfigureAwait(false))
            {
                _logger.LogInformation("Table {Table} not found, returning empty list.", tableName);
                return [];
            }

            return await LoadTableAsync(datasource, tableName, mapper, cancellationToken).ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            _logger.LogInformation("Table {Table} not found, returning empty list.", tableName);
            return [];
        }
    }
}
