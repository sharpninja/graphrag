// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;
using Microsoft.AspNetCore.Components;

namespace GraphRag.SearchApp.Components;

/// <summary>
/// Displays the details of a community report.
/// </summary>
public partial class ReportDetails
{
    /// <summary>
    /// Gets or sets the community report to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public CommunityReport Report { get; set; } = default!;
}
