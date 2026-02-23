// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage.Tables;

namespace GraphRag.DataModel;

/// <summary>
/// Reads typed data model objects from a table provider.
/// </summary>
public class DataReader
{
    private readonly ITableProvider _tableProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataReader"/> class.
    /// </summary>
    /// <param name="tableProvider">The table provider to read data from.</param>
    public DataReader(ITableProvider tableProvider)
    {
        ArgumentNullException.ThrowIfNull(tableProvider);
        _tableProvider = tableProvider;
    }

    /// <summary>
    /// Reads entities from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of entities.</returns>
    public async Task<IReadOnlyList<Entity>> EntitiesAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_entities", cancellationToken).ConfigureAwait(false);
        return rows.Select(Entity.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads relationships from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of relationships.</returns>
    public async Task<IReadOnlyList<Relationship>> RelationshipsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_relationships", cancellationToken).ConfigureAwait(false);
        return rows.Select(Relationship.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads communities from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of communities.</returns>
    public async Task<IReadOnlyList<Community>> CommunitiesAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_communities", cancellationToken).ConfigureAwait(false);
        return rows.Select(Community.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads community reports from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of community reports.</returns>
    public async Task<IReadOnlyList<CommunityReport>> CommunityReportsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_community_reports", cancellationToken).ConfigureAwait(false);
        return rows.Select(CommunityReport.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads covariates from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of covariates.</returns>
    public async Task<IReadOnlyList<Covariate>> CovariatesAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_covariates", cancellationToken).ConfigureAwait(false);
        return rows.Select(Covariate.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads text units from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of text units.</returns>
    public async Task<IReadOnlyList<TextUnit>> TextUnitsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_text_units", cancellationToken).ConfigureAwait(false);
        return rows.Select(TextUnit.FromDictionary).ToList();
    }

    /// <summary>
    /// Reads documents from storage.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of documents.</returns>
    public async Task<IReadOnlyList<Document>> DocumentsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _tableProvider.ReadAsync("create_final_documents", cancellationToken).ConfigureAwait(false);
        return rows.Select(Document.FromDictionary).ToList();
    }
}
