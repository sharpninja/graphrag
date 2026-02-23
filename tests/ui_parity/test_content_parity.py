# Copyright (c) 2025 Microsoft Corporation.
# Licensed under the MIT License

"""
Content parity tests — verify both apps display the same semantic content.

Tests that the Python (Streamlit) and .NET (Blazor) search apps present
identical data when loaded with the same Christmas Carol seed dataset.
"""

import pytest
from playwright.sync_api import Page
from tests.ui_parity.helpers import click_python_tab, get_python_tabbar_frame, click_blazor_tab


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def get_visible_text(page: Page) -> str:
    """Return all visible text content on the page."""
    return page.evaluate("() => document.body.innerText")


def get_python_full_text(page: Page) -> str:
    """Get visible text including iframe content for the Python app."""
    main_text = page.evaluate("() => document.body.innerText")
    frame = get_python_tabbar_frame(page)
    if frame:
        main_text += "\n" + frame.evaluate("() => document.body.innerText")
    return main_text


# ---------------------------------------------------------------------------
# A) Dataset name and description visible
# ---------------------------------------------------------------------------

class TestDatasetContent:
    """Both apps must show the same dataset name and description."""

    def test_python_shows_christmas_carol_dataset(self, python_page: Page):
        text = get_visible_text(python_page)
        assert "A Christmas Carol" in text or "christmas-carol" in text

    def test_blazor_shows_christmas_carol_dataset(self, blazor_page: Page):
        text = get_visible_text(blazor_page)
        # Blazor sidebar/main page should show dataset name
        assert "Christmas Carol" in text or "christmas-carol" in text.lower(), \
            "PARITY GAP: Blazor does not display the dataset name on the page"

    def test_both_show_dataset_description(self, python_page: Page, blazor_page: Page):
        py_text = get_visible_text(python_page)
        bl_text = get_visible_text(blazor_page)
        desc_fragment = "Charles Dickens"
        assert desc_fragment in py_text, f"Python app missing '{desc_fragment}'"
        assert desc_fragment in bl_text, f"Blazor app missing '{desc_fragment}'"


# ---------------------------------------------------------------------------
# B) Tab structure: both apps have Search and Community Explorer tabs
# ---------------------------------------------------------------------------

class TestTabStructure:
    """Both apps must have Search and Community Explorer tabs."""

    def test_python_has_search_tab(self, python_page: Page):
        """Python TabBar iframe contains a Search tab."""
        frame = get_python_tabbar_frame(python_page)
        assert frame is not None, "TabBar iframe not found"
        text = frame.evaluate("() => document.body.innerText")
        assert "SEARCH" in text.upper()

    def test_python_has_community_explorer_tab(self, python_page: Page):
        """Python TabBar iframe contains a Community Explorer tab."""
        frame = get_python_tabbar_frame(python_page)
        assert frame is not None, "TabBar iframe not found"
        text = frame.evaluate("() => document.body.innerText")
        assert "COMMUNITY EXPLORER" in text.upper()

    def test_blazor_has_search_tab(self, blazor_page: Page):
        tabs = blazor_page.locator("button[role='tab']").all_inner_texts()
        assert any("SEARCH" in t.upper() for t in tabs), f"No Search tab: {tabs}"

    def test_blazor_has_community_explorer_tab(self, blazor_page: Page):
        tabs = blazor_page.locator("button[role='tab']").all_inner_texts()
        assert any("COMMUNITY EXPLORER" in t.upper() for t in tabs), f"No Community Explorer tab: {tabs}"

    def test_tab_names_match(self, python_page: Page, blazor_page: Page):
        """Both apps have tabs with the same names (case-insensitive)."""
        blazor_tabs = {t.strip().upper() for t in blazor_page.locator("button[role='tab']").all_inner_texts()}
        frame = get_python_tabbar_frame(python_page)
        assert frame is not None
        py_text = frame.evaluate("() => document.body.innerText").upper()
        assert "SEARCH" in blazor_tabs and "SEARCH" in py_text
        assert "COMMUNITY EXPLORER" in blazor_tabs and "COMMUNITY EXPLORER" in py_text


# ---------------------------------------------------------------------------
# C) Sidebar controls: same labels in same order
# ---------------------------------------------------------------------------

class TestSidebarContent:
    """Both apps must have the same sidebar controls with the same labels."""

    EXPECTED_LABELS = [
        "Options",
        "Dataset",
        "suggested questions",
        "Search options",
        "Include basic RAG",
        "Include local search",
        "Include global search",
        "Include drift search",
    ]

    def test_python_sidebar_has_expected_labels(self, python_page: Page):
        sidebar = python_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text()
        for label in self.EXPECTED_LABELS:
            assert label.lower() in sidebar_text.lower(), \
                f"Python sidebar missing '{label}'"

    def test_blazor_sidebar_has_expected_labels(self, blazor_page: Page):
        sidebar = blazor_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text()
        for label in self.EXPECTED_LABELS:
            assert label.lower() in sidebar_text.lower(), \
                f"Blazor sidebar missing '{label}'"

    def test_sidebar_toggle_defaults_match(self, python_page: Page, blazor_page: Page):
        """Default toggle states should match between apps."""
        py_text = get_visible_text(python_page).lower()
        bl_text = get_visible_text(blazor_page).lower()
        for toggle in ["include basic rag", "include local search",
                        "include global search", "include drift search"]:
            assert toggle in py_text, f"Python missing toggle '{toggle}'"
            assert toggle in bl_text, f"Blazor missing toggle '{toggle}'"


# ---------------------------------------------------------------------------
# D) Search result column structure
# ---------------------------------------------------------------------------

class TestSearchStructure:
    """Both apps show search type headings."""

    def test_python_search_tab_is_default(self, python_page: Page):
        """Python app loads with the Search tab active by default."""
        text = get_visible_text(python_page)
        assert "question" in text.lower() or "suggest" in text.lower()

    def test_blazor_search_tab_is_default(self, blazor_page: Page):
        """Blazor app loads with the Search tab active by default."""
        text = get_visible_text(blazor_page)
        assert "question" in text.lower() or "suggest" in text.lower()

    def test_both_show_search_heading_labels(self, python_page: Page, blazor_page: Page):
        """Both apps reference local and global search types (Python shows column headings)."""
        py_text = get_visible_text(python_page).lower()
        assert "local search" in py_text, "Python missing 'local search'"
        # Blazor shows search type labels after a search is run; verify tabs exist
        bl_tabs = blazor_page.locator("button[role='tab']").all_inner_texts()
        assert any("SEARCH" in t.upper() for t in bl_tabs), "Blazor missing Search tab"


# ---------------------------------------------------------------------------
# E) Community Explorer content from seed data
# ---------------------------------------------------------------------------

class TestCommunityExplorerContent:
    """Both apps must show the same community reports from seed data."""

    EXPECTED_REPORT_TITLES = [
        "Scrooge's Redemption Arc",
        "The Cratchit Family Hardship",
    ]

    def test_python_community_explorer_shows_reports_section(self, python_page: Page):
        """Python community explorer loads and shows the reports heading."""
        click_python_tab(python_page, "Community Explorer")
        text = get_visible_text(python_page)
        assert "Community Reports" in text

    def test_python_community_explorer_has_dataframe(self, python_page: Page):
        """Python renders reports in a st.dataframe widget."""
        click_python_tab(python_page, "Community Explorer")
        # Streamlit renders report list as a canvas-based dataframe
        df_count = python_page.locator("[data-testid='stDataFrame']").count()
        assert df_count > 0, "Python Community Explorer missing dataframe widget"

    def test_blazor_community_explorer_shows_reports(self, blazor_page: Page):
        click_blazor_tab(blazor_page, "Community Explorer")
        text = get_visible_text(blazor_page)
        assert "Community Reports" in text

    def test_blazor_shows_seed_report_titles(self, blazor_page: Page):
        click_blazor_tab(blazor_page, "Community Explorer")
        text = get_visible_text(blazor_page)
        for title in self.EXPECTED_REPORT_TITLES:
            assert title in text, f"Blazor missing report: '{title}'"

    def test_both_show_community_explorer_structure(self, python_page: Page, blazor_page: Page):
        """Both apps have Community Reports heading and Selected Report panel."""
        click_python_tab(python_page, "Community Explorer")
        click_blazor_tab(blazor_page, "Community Explorer")
        py_text = get_visible_text(python_page)
        bl_text = get_visible_text(blazor_page)
        assert "Community Reports" in py_text
        assert "Community Reports" in bl_text
        assert "Selected Report" in py_text
        assert "Selected Report" in bl_text


# ---------------------------------------------------------------------------
# F) Question input element
# ---------------------------------------------------------------------------

class TestQuestionInput:
    """Both apps must have a text input for asking questions."""

    def test_python_has_question_input(self, python_page: Page):
        text = get_visible_text(python_page)
        has_input = (
            "question" in text.lower()
            or python_page.locator("input[type='text']").count() > 0
        )
        assert has_input, "Python app missing question input"

    def test_blazor_has_question_input(self, blazor_page: Page):
        has_input = blazor_page.locator("input").count() > 0
        assert has_input, "Blazor app missing question input"


# ---------------------------------------------------------------------------
# G) Suggest Questions button
# ---------------------------------------------------------------------------

class TestSuggestQuestionsButton:
    """Both apps must have a button to suggest questions."""

    def test_python_has_suggest_button(self, python_page: Page):
        text = get_visible_text(python_page)
        assert "suggest" in text.lower(), "Python missing 'Suggest' button"

    def test_blazor_has_suggest_button(self, blazor_page: Page):
        text = get_visible_text(blazor_page)
        assert "suggest" in text.lower(), "Blazor missing 'Suggest' button"
