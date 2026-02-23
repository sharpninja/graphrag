// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;
using GraphRag.SearchApp.Config;
using Parquet;
using Parquet.Schema;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Generates demo seed data (parquet files) for the SearchApp.
/// Creates a "Christmas Carol" sample dataset based on the GraphRAG getting-started example.
/// </summary>
public static class SeedDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Seeds the data directory with demo data if no listing file exists.
    /// </summary>
    /// <param name="config">The search app configuration.</param>
    /// <param name="force">When true, overwrites existing data.</param>
    /// <returns>A task representing the async operation.</returns>
    public static async Task SeedAsync(SearchAppConfig config, bool force = false)
    {
        ArgumentNullException.ThrowIfNull(config);

        var listingPath = Path.Combine(config.DataRoot, config.ListingFile);
        if (File.Exists(listingPath) && !force)
        {
            Console.WriteLine($"Seed data already exists at {listingPath}. Use --force to overwrite.");
            return;
        }

        Console.WriteLine($"Seeding demo data to {config.DataRoot}...");

        var datasetPath = Path.Combine(config.DataRoot, "christmas-carol");
        var outputPath = Path.Combine(datasetPath, "output");
        Directory.CreateDirectory(outputPath);

        // Write all parquet tables in parallel
        await Task.WhenAll(
            WriteEntitiesAsync(outputPath),
            WriteRelationshipsAsync(outputPath),
            WriteCommunitiesAsync(outputPath),
            WriteCommunityReportsAsync(outputPath),
            WriteTextUnitsAsync(outputPath)).ConfigureAwait(false);

        // Write listing.json
        var listing = new[]
        {
            new
            {
                key = "christmas-carol",
                path = "christmas-carol",
                name = "A Christmas Carol",
                description = "GraphRAG index of Charles Dickens' A Christmas Carol — demo dataset for the SearchApp.",
                communityLevel = 1,
            },
        };

        await File.WriteAllTextAsync(
            listingPath,
            JsonSerializer.Serialize(listing, JsonOptions)).ConfigureAwait(false);

        Console.WriteLine($"Seed data created: {listingPath}");
        Console.WriteLine($"  Dataset: christmas-carol ({outputPath})");
        Console.WriteLine("  Tables: entities, relationships, communities, community_reports, text_units");
    }

    private static async Task WriteEntitiesAsync(string outputPath)
    {
        var schema = new ParquetSchema(
            new DataField<string>("id"),
            new DataField<string>("short_id"),
            new DataField<string>("title"),
            new DataField<string>("type"),
            new DataField<string>("description"),
            new DataField<int>("rank"));

        string[] ids = ["e-0", "e-1", "e-2", "e-3", "e-4", "e-5", "e-6", "e-7"];
        string[] shortIds = ["0", "1", "2", "3", "4", "5", "6", "7"];
        string[] titles =
        [
            "EBENEZER SCROOGE", "BOB CRATCHIT", "TINY TIM", "JACOB MARLEY",
            "GHOST OF CHRISTMAS PAST", "GHOST OF CHRISTMAS PRESENT", "GHOST OF CHRISTMAS YET TO COME", "FRED",
        ];
        string[] types = ["PERSON", "PERSON", "PERSON", "PERSON", "ENTITY", "ENTITY", "ENTITY", "PERSON"];
        string[] descriptions =
        [
            "Ebenezer Scrooge is a miserly London businessman who despises Christmas and all forms of generosity. Through supernatural visitation he undergoes a dramatic moral transformation.",
            "Bob Cratchit is Scrooge's loyal and underpaid clerk who endures harsh working conditions with patience and maintains a loving household despite poverty.",
            "Tiny Tim is Bob Cratchit's youngest son, a small sickly child who walks with a crutch. His potential death serves as a catalyst for Scrooge's change of heart.",
            "Jacob Marley is Scrooge's deceased business partner whose ghost appears bound in heavy chains, warning Scrooge of the consequences of a life devoted solely to greed.",
            "The Ghost of Christmas Past is the first of three spirits to visit Scrooge, showing him scenes from his earlier life that shaped his current miserly disposition.",
            "The Ghost of Christmas Present is a jovial giant spirit who reveals to Scrooge the current Christmas celebrations of those around him, including the Cratchit family.",
            "The Ghost of Christmas Yet to Come is a dark, silent phantom who shows Scrooge visions of a grim future, including his own lonely death and Tiny Tim's demise.",
            "Fred is Scrooge's cheerful nephew who persistently invites his uncle to Christmas dinner despite repeated rebuffs, embodying the generous Christmas spirit.",
        ];
        int[] ranks = [10, 7, 8, 6, 5, 5, 5, 4];

        var filePath = Path.Combine(outputPath, "entities.parquet");
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream).ConfigureAwait(false);
        using var group = writer.CreateRowGroup();
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[0], ids)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[1], shortIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[2], titles)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[3], types)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[4], descriptions)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[5], ranks)).ConfigureAwait(false);
    }

    private static async Task WriteRelationshipsAsync(string outputPath)
    {
        var schema = new ParquetSchema(
            new DataField<string>("id"),
            new DataField<string>("short_id"),
            new DataField<string>("source"),
            new DataField<string>("target"),
            new DataField<double>("weight"),
            new DataField<string>("description"));

        string[] ids = ["r-0", "r-1", "r-2", "r-3", "r-4", "r-5", "r-6", "r-7", "r-8", "r-9"];
        string[] shortIds = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
        string[] sources =
        [
            "EBENEZER SCROOGE", "EBENEZER SCROOGE", "BOB CRATCHIT", "JACOB MARLEY",
            "GHOST OF CHRISTMAS PAST", "GHOST OF CHRISTMAS PRESENT", "GHOST OF CHRISTMAS YET TO COME",
            "EBENEZER SCROOGE", "GHOST OF CHRISTMAS PRESENT", "EBENEZER SCROOGE",
        ];
        string[] targets =
        [
            "BOB CRATCHIT", "JACOB MARLEY", "TINY TIM", "EBENEZER SCROOGE",
            "EBENEZER SCROOGE", "EBENEZER SCROOGE", "EBENEZER SCROOGE",
            "FRED", "TINY TIM", "TINY TIM",
        ];
        double[] weights = [8.0, 9.0, 10.0, 9.0, 7.0, 7.0, 8.0, 5.0, 6.0, 7.0];
        string[] descriptions =
        [
            "Scrooge employs Cratchit as his clerk and treats him poorly, reflecting his miserly nature",
            "Marley was Scrooge's business partner; his ghost warns Scrooge to change his ways",
            "Tiny Tim is Cratchit's beloved youngest son who is gravely ill",
            "Marley's ghost appears to Scrooge on Christmas Eve, initiating his transformation",
            "The Ghost of Christmas Past takes Scrooge on a journey through his earlier memories",
            "The Ghost of Christmas Present shows Scrooge how others celebrate the holiday season",
            "The Ghost of Christmas Yet to Come reveals a dark future if Scrooge does not change",
            "Fred is Scrooge's nephew who repeatedly invites him to Christmas dinner",
            "The Ghost shows Scrooge the Cratchit family and Tiny Tim's fragile condition",
            "Scrooge becomes deeply moved by Tiny Tim's plight, catalyzing his redemption",
        ];

        var filePath = Path.Combine(outputPath, "relationships.parquet");
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream).ConfigureAwait(false);
        using var group = writer.CreateRowGroup();
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[0], ids)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[1], shortIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[2], sources)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[3], targets)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[4], weights)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[5], descriptions)).ConfigureAwait(false);
    }

    private static async Task WriteCommunitiesAsync(string outputPath)
    {
        var schema = new ParquetSchema(
            new DataField<string>("id"),
            new DataField<string>("short_id"),
            new DataField<string>("title"),
            new DataField<string>("level"),
            new DataField<string>("parent"),
            new DataField<int>("size"));

        string[] ids = ["c-0", "c-1"];
        string[] shortIds = ["0", "1"];
        string[] titles = ["Scrooge's Transformation", "The Cratchit Family"];
        string[] levels = ["0", "0"];
        string[] parents = [string.Empty, string.Empty];
        int[] sizes = [5, 3];

        var filePath = Path.Combine(outputPath, "communities.parquet");
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream).ConfigureAwait(false);
        using var group = writer.CreateRowGroup();
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[0], ids)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[1], shortIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[2], titles)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[3], levels)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[4], parents)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[5], sizes)).ConfigureAwait(false);
    }

    private static async Task WriteCommunityReportsAsync(string outputPath)
    {
        var schema = new ParquetSchema(
            new DataField<string>("id"),
            new DataField<string>("short_id"),
            new DataField<string>("title"),
            new DataField<string>("community_id"),
            new DataField<string>("summary"),
            new DataField<string>("full_content"),
            new DataField<double>("rank"),
            new DataField<int>("size"));

        string[] ids = ["cr-0", "cr-1"];
        string[] shortIds = ["0", "1"];
        string[] titles = ["Scrooge's Redemption Arc", "The Cratchit Family Hardship"];
        string[] communityIds = ["0", "1"];
        string[] summaries =
        [
            "This community centers on Ebenezer Scrooge and the supernatural forces that drive his transformation from miser to philanthropist. Key entities include Jacob Marley's ghost and the three Christmas spirits who guide Scrooge through visions of past, present, and future.",
            "This community revolves around the Cratchit family, particularly Bob Cratchit and his son Tiny Tim. Despite severe poverty caused by Scrooge's miserly wages, the family maintains warmth and love, serving as a moral counterpoint to Scrooge's isolation.",
        ];
        string[] fullContents =
        [
            @"# Scrooge's Redemption Arc

## Overview
The central narrative of A Christmas Carol follows Ebenezer Scrooge's dramatic moral transformation on Christmas Eve. Scrooge begins as a cold, miserly businessman who values profit above all else and despises the Christmas season.

## Key Findings

1. **Supernatural Intervention**: Jacob Marley's ghost initiates the transformation by warning Scrooge about the consequences of greed. Marley appears bound in heavy chains forged during his lifetime of avarice.

2. **Journey Through Time**: Three spirits — the Ghost of Christmas Past, the Ghost of Christmas Present, and the Ghost of Christmas Yet to Come — each reveal crucial truths that chip away at Scrooge's hardened exterior.

3. **Pivotal Moment**: The vision of Tiny Tim's empty chair and small crutch in the future proves to be the decisive emotional turning point for Scrooge.

4. **Complete Transformation**: By Christmas morning, Scrooge has undergone a total reversal — he sends a prize turkey to the Cratchits, raises Bob's salary, and becomes a second father to Tiny Tim.",
            @"# The Cratchit Family Hardship

## Overview
The Cratchit family represents the working poor of Victorian London. Bob Cratchit earns a meager fifteen shillings a week from Scrooge's counting house, barely enough to feed his large family.

## Key Findings

1. **Resilience and Love**: Despite their poverty, the Cratchit family celebrates Christmas with genuine joy and gratitude, contrasting sharply with Scrooge's wealth and misery.

2. **Tiny Tim's Illness**: Tiny Tim's physical disability and illness create urgency in the narrative. Without improved circumstances, the Ghost of Christmas Present implies Tim will not survive.

3. **Bob's Loyalty**: Bob Cratchit remains loyal and respectful toward Scrooge despite the harsh working conditions, even proposing a toast to his employer during Christmas dinner.

4. **Catalyst for Change**: The Cratchit family's circumstances, particularly Tiny Tim's plight, serve as the primary emotional catalyst for Scrooge's redemption.",
        ];
        double[] ranks = [9.5, 8.0];
        int[] sizes = [5, 3];

        var filePath = Path.Combine(outputPath, "community_reports.parquet");
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream).ConfigureAwait(false);
        using var group = writer.CreateRowGroup();
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[0], ids)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[1], shortIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[2], titles)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[3], communityIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[4], summaries)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[5], fullContents)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[6], ranks)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[7], sizes)).ConfigureAwait(false);
    }

    private static async Task WriteTextUnitsAsync(string outputPath)
    {
        var schema = new ParquetSchema(
            new DataField<string>("id"),
            new DataField<string>("short_id"),
            new DataField<string>("text"),
            new DataField<int>("n_tokens"),
            new DataField<string>("document_id"));

        string[] ids = ["tu-0", "tu-1", "tu-2", "tu-3", "tu-4"];
        string[] shortIds = ["0", "1", "2", "3", "4"];
        string[] texts =
        [
            "Marley was dead: to begin with. There is no doubt whatever about that. The register of his burial was signed by the clergyman, the clerk, the undertaker, and the chief mourner. Scrooge signed it: and Scrooge's name was good upon 'Change, for anything he chose to put his hand to. Old Marley was as dead as a door-nail.",
            "Oh! But he was a tight-fisted hand at the grindstone, Scrooge! a squeezing, wrenching, grasping, scraping, clutching, covetous, old sinner! Hard and sharp as flint, from which no steel had ever struck out generous fire; secret, and self-contained, and solitary as an oyster.",
            "'A merry Christmas, uncle! God save you!' cried a cheerful voice. It was the voice of Scrooge's nephew, who came upon him so quickly that this was the first intimation he had of his approach. 'Bah!' said Scrooge, 'Humbug!'",
            "Bob Cratchit's fire was so very much smaller that it looked like one coal. But he couldn't replenish it, for Scrooge kept the coal-box in his own room. Wherefore the clerk put on his white comforter, and tried to warm himself at the candle.",
            "'God bless us every one!' said Tiny Tim, the last of all. He sat very close to his father's side upon his little stool. Bob held his withered little hand in his, as if he loved the child, and wished to keep him by his side, and dreaded that he might be taken from him.",
        ];
        int[] nTokens = [68, 52, 53, 47, 52];
        string[] documentIds = ["doc-0", "doc-0", "doc-0", "doc-0", "doc-0"];

        var filePath = Path.Combine(outputPath, "text_units.parquet");
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream).ConfigureAwait(false);
        using var group = writer.CreateRowGroup();
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[0], ids)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[1], shortIds)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[2], texts)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[3], nTokens)).ConfigureAwait(false);
        await group.WriteColumnAsync(new Parquet.Data.DataColumn(schema.DataFields[4], documentIds)).ConfigureAwait(false);
    }
}
