// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.SearchApp.Config;
using GraphRag.SearchApp.Services;
using GraphRag.SearchApp.ViewModels;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<SearchAppConfig>(
    builder.Configuration.GetSection("SearchApp"));

// MudBlazor
builder.Services.AddMudServices();

// Razor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Application services
builder.Services.AddScoped<MarkdownRenderer>();
builder.Services.AddScoped<DatasetLoader>();
builder.Services.AddScoped<KnowledgeModelService>();
builder.Services.AddScoped<SearchOrchestrator>();

// ViewModels (scoped per Blazor circuit)
builder.Services.AddScoped<AppStateViewModel>();
builder.Services.AddScoped<SearchViewModel>();
builder.Services.AddScoped<CommunityExplorerViewModel>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<GraphRag.SearchApp.App>()
    .AddInteractiveServerRenderMode();

app.Run();
