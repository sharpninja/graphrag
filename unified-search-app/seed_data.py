# Copyright (c) 2025 Microsoft Corporation.
# Licensed under the MIT License

"""
Seed data generator for the GraphRAG Unified Search App.

Creates a "Christmas Carol" demo dataset with Parquet files matching the
GraphRAG indexing output format. Identical data to the .NET SeedDataService.

Usage:
    python seed_data.py [--force] [--data-root /path/to/output]
"""

import argparse
import json
import os
import sys

import pandas as pd


def seed(data_root: str, force: bool = False) -> None:
    """Generate demo seed data in the specified data root directory."""
    listing_path = os.path.join(data_root, "listing.json")

    if os.path.exists(listing_path) and not force:
        print(f"Seed data already exists at {listing_path}. Use --force to overwrite.")
        return

    print(f"Seeding demo data to {data_root}...")

    dataset_path = os.path.join(data_root, "christmas-carol")
    output_path = os.path.join(dataset_path, "output")
    os.makedirs(output_path, exist_ok=True)

    _write_entities(output_path)
    _write_relationships(output_path)
    _write_communities(output_path)
    _write_community_reports(output_path)
    _write_text_units(output_path)
    _write_settings(dataset_path)

    # Write listing.json
    listing = [
        {
            "key": "christmas-carol",
            "path": "christmas-carol",
            "name": "A Christmas Carol",
            "description": "GraphRAG index of Charles Dickens' A Christmas Carol — demo dataset for the SearchApp.",
            "community_level": 1,
        }
    ]
    with open(listing_path, "w") as f:
        json.dump(listing, f, indent=2)

    print(f"Seed data created: {listing_path}")
    print(f"  Dataset: christmas-carol ({output_path})")
    print("  Tables: entities, relationships, communities, community_reports, text_units")


def _write_entities(output_path: str) -> None:
    df = pd.DataFrame({
        "id": ["e-0", "e-1", "e-2", "e-3", "e-4", "e-5", "e-6", "e-7"],
        "short_id": ["0", "1", "2", "3", "4", "5", "6", "7"],
        "title": [
            "EBENEZER SCROOGE", "BOB CRATCHIT", "TINY TIM", "JACOB MARLEY",
            "GHOST OF CHRISTMAS PAST", "GHOST OF CHRISTMAS PRESENT",
            "GHOST OF CHRISTMAS YET TO COME", "FRED",
        ],
        "type": ["PERSON", "PERSON", "PERSON", "PERSON", "ENTITY", "ENTITY", "ENTITY", "PERSON"],
        "description": [
            "Ebenezer Scrooge is a miserly London businessman who despises Christmas and all forms of generosity. Through supernatural visitation he undergoes a dramatic moral transformation.",
            "Bob Cratchit is Scrooge's loyal and underpaid clerk who endures harsh working conditions with patience and maintains a loving household despite poverty.",
            "Tiny Tim is Bob Cratchit's youngest son, a small sickly child who walks with a crutch. His potential death serves as a catalyst for Scrooge's change of heart.",
            "Jacob Marley is Scrooge's deceased business partner whose ghost appears bound in heavy chains, warning Scrooge of the consequences of a life devoted solely to greed.",
            "The Ghost of Christmas Past is the first of three spirits to visit Scrooge, showing him scenes from his earlier life that shaped his current miserly disposition.",
            "The Ghost of Christmas Present is a jovial giant spirit who reveals to Scrooge the current Christmas celebrations of those around him, including the Cratchit family.",
            "The Ghost of Christmas Yet to Come is a dark, silent phantom who shows Scrooge visions of a grim future, including his own lonely death and Tiny Tim's demise.",
            "Fred is Scrooge's cheerful nephew who persistently invites his uncle to Christmas dinner despite repeated rebuffs, embodying the generous Christmas spirit.",
        ],
        "rank": [10, 7, 8, 6, 5, 5, 5, 4],
    })
    df.to_parquet(os.path.join(output_path, "entities.parquet"), index=False)


def _write_relationships(output_path: str) -> None:
    df = pd.DataFrame({
        "id": ["r-0", "r-1", "r-2", "r-3", "r-4", "r-5", "r-6", "r-7", "r-8", "r-9"],
        "short_id": ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
        "source": [
            "EBENEZER SCROOGE", "EBENEZER SCROOGE", "BOB CRATCHIT", "JACOB MARLEY",
            "GHOST OF CHRISTMAS PAST", "GHOST OF CHRISTMAS PRESENT",
            "GHOST OF CHRISTMAS YET TO COME", "EBENEZER SCROOGE",
            "GHOST OF CHRISTMAS PRESENT", "EBENEZER SCROOGE",
        ],
        "target": [
            "BOB CRATCHIT", "JACOB MARLEY", "TINY TIM", "EBENEZER SCROOGE",
            "EBENEZER SCROOGE", "EBENEZER SCROOGE", "EBENEZER SCROOGE",
            "FRED", "TINY TIM", "TINY TIM",
        ],
        "weight": [8.0, 9.0, 10.0, 9.0, 7.0, 7.0, 8.0, 5.0, 6.0, 7.0],
        "description": [
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
        ],
    })
    df.to_parquet(os.path.join(output_path, "relationships.parquet"), index=False)


def _write_communities(output_path: str) -> None:
    df = pd.DataFrame({
        "id": ["c-0", "c-1"],
        "short_id": ["0", "1"],
        "title": ["Scrooge's Transformation", "The Cratchit Family"],
        "level": ["0", "0"],
        "parent": ["", ""],
        "size": [5, 3],
    })
    df.to_parquet(os.path.join(output_path, "communities.parquet"), index=False)


def _write_community_reports(output_path: str) -> None:
    df = pd.DataFrame({
        "id": ["cr-0", "cr-1"],
        "short_id": ["0", "1"],
        "title": ["Scrooge's Redemption Arc", "The Cratchit Family Hardship"],
        "community_id": ["0", "1"],
        "summary": [
            "This community centers on Ebenezer Scrooge and the supernatural forces that drive his transformation from miser to philanthropist. Key entities include Jacob Marley's ghost and the three Christmas spirits who guide Scrooge through visions of past, present, and future.",
            "This community revolves around the Cratchit family, particularly Bob Cratchit and his son Tiny Tim. Despite severe poverty caused by Scrooge's miserly wages, the family maintains warmth and love, serving as a moral counterpoint to Scrooge's isolation.",
        ],
        "full_content": [
            """# Scrooge's Redemption Arc

## Overview
The central narrative of A Christmas Carol follows Ebenezer Scrooge's dramatic moral transformation on Christmas Eve. Scrooge begins as a cold, miserly businessman who values profit above all else and despises the Christmas season.

## Key Findings

1. **Supernatural Intervention**: Jacob Marley's ghost initiates the transformation by warning Scrooge about the consequences of greed. Marley appears bound in heavy chains forged during his lifetime of avarice.

2. **Journey Through Time**: Three spirits — the Ghost of Christmas Past, the Ghost of Christmas Present, and the Ghost of Christmas Yet to Come — each reveal crucial truths that chip away at Scrooge's hardened exterior.

3. **Pivotal Moment**: The vision of Tiny Tim's empty chair and small crutch in the future proves to be the decisive emotional turning point for Scrooge.

4. **Complete Transformation**: By Christmas morning, Scrooge has undergone a total reversal — he sends a prize turkey to the Cratchits, raises Bob's salary, and becomes a second father to Tiny Tim.""",
            """# The Cratchit Family Hardship

## Overview
The Cratchit family represents the working poor of Victorian London. Bob Cratchit earns a meager fifteen shillings a week from Scrooge's counting house, barely enough to feed his large family.

## Key Findings

1. **Resilience and Love**: Despite their poverty, the Cratchit family celebrates Christmas with genuine joy and gratitude, contrasting sharply with Scrooge's wealth and misery.

2. **Tiny Tim's Illness**: Tiny Tim's physical disability and illness create urgency in the narrative. Without improved circumstances, the Ghost of Christmas Present implies Tim will not survive.

3. **Bob's Loyalty**: Bob Cratchit remains loyal and respectful toward Scrooge despite the harsh working conditions, even proposing a toast to his employer during Christmas dinner.

4. **Catalyst for Change**: The Cratchit family's circumstances, particularly Tiny Tim's plight, serve as the primary emotional catalyst for Scrooge's redemption.""",
        ],
        "rank": [9.5, 8.0],
        "size": [5, 3],
    })
    df.to_parquet(os.path.join(output_path, "community_reports.parquet"), index=False)


def _write_text_units(output_path: str) -> None:
    df = pd.DataFrame({
        "id": ["tu-0", "tu-1", "tu-2", "tu-3", "tu-4"],
        "short_id": ["0", "1", "2", "3", "4"],
        "text": [
            "Marley was dead: to begin with. There is no doubt whatever about that. The register of his burial was signed by the clergyman, the clerk, the undertaker, and the chief mourner. Scrooge signed it: and Scrooge's name was good upon 'Change, for anything he chose to put his hand to. Old Marley was as dead as a door-nail.",
            "Oh! But he was a tight-fisted hand at the grindstone, Scrooge! a squeezing, wrenching, grasping, scraping, clutching, covetous, old sinner! Hard and sharp as flint, from which no steel had ever struck out generous fire; secret, and self-contained, and solitary as an oyster.",
            "'A merry Christmas, uncle! God save you!' cried a cheerful voice. It was the voice of Scrooge's nephew, who came upon him so quickly that this was the first intimation he had of his approach. 'Bah!' said Scrooge, 'Humbug!'",
            "Bob Cratchit's fire was so very much smaller that it looked like one coal. But he couldn't replenish it, for Scrooge kept the coal-box in his own room. Wherefore the clerk put on his white comforter, and tried to warm himself at the candle.",
            "'God bless us every one!' said Tiny Tim, the last of all. He sat very close to his father's side upon his little stool. Bob held his withered little hand in his, as if he loved the child, and wished to keep him by his side, and dreaded that he might be taken from him.",
        ],
        "n_tokens": [68, 52, 53, 47, 52],
        "document_id": ["doc-0", "doc-0", "doc-0", "doc-0", "doc-0"],
    })
    df.to_parquet(os.path.join(output_path, "text_units.parquet"), index=False)


def _write_settings(dataset_path: str) -> None:
    """Write a minimal settings.yaml so load_config succeeds."""
    import yaml

    settings = {
        "models": {
            "default_chat_model": {
                "type": "openai_chat",
                "api_key": "sk-demo-placeholder",
                "model": "gpt-4",
            },
            "default_embedding_model": {
                "type": "openai_embedding",
                "api_key": "sk-demo-placeholder",
                "model": "text-embedding-3-small",
            },
        }
    }
    settings_path = os.path.join(dataset_path, "settings.yaml")
    with open(settings_path, "w") as f:
        yaml.dump(settings, f, default_flow_style=False)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Seed demo data for the GraphRAG Search App")
    parser.add_argument(
        "--data-root",
        default=os.getenv("DATA_ROOT", "./data"),
        help="Root directory for data output (default: $DATA_ROOT or ./data)",
    )
    parser.add_argument(
        "--force",
        action="store_true",
        help="Overwrite existing seed data",
    )
    args = parser.parse_args()
    seed(args.data_root, args.force)
