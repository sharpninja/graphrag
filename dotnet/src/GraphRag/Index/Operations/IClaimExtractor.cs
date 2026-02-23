// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.Index.Operations;

/// <summary>
/// Extracts claims (covariates) from text.
/// </summary>
public interface IClaimExtractor
{
    /// <summary>
    /// Extracts covariates from the given text.
    /// </summary>
    /// <param name="text">The input text to extract claims from.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A list of extracted covariates.</returns>
    Task<IReadOnlyList<Covariate>> ExtractAsync(string text, CancellationToken ct);
}
