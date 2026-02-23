# Copyright (c) 2025 Microsoft Corporation.
# Licensed under the MIT License

"""
Behavior parity tests — verify both apps respond identically to user interactions.

Tests that the Python (Streamlit) and .NET (Blazor) search apps behave
the same way when users interact with tabs, reports, and controls.
"""

import pytest
from playwright.sync_api import Page
from tests.ui_parity.helpers import click_python_tab, get_python_tabbar_frame, click_blazor_tab


def get_visible_text(page: Page) -> str:
    """Return all visible text content on the page."""
    return page.evaluate("() => document.body.innerText")


# ---------------------------------------------------------------------------
# A) Tab switching behavior
# ---------------------------------------------------------------------------

class TestTabSwitching:
    """Both apps must support switching between Search and Community Explorer."""

    def test_python_tab_switch_to_community(self, python_page: Page):
        """Clicking Community Explorer tab shows community content."""
        click_python_tab(python_page, "Community Explorer")
        text = get_visible_text(python_page)
        assert "Community Reports" in text

    def test_python_tab_switch_back_to_search(self, python_page: Page):
        """Clicking Search tab after Community Explorer returns to search."""
        click_python_tab(python_page, "Community Explorer")
        click_python_tab(python_page, "Search")
        text = get_visible_text(python_page)
        assert "question" in text.lower() or "suggest" in text.lower()

    def test_blazor_tab_switch_to_community(self, blazor_page: Page):
        """Clicking Community Explorer tab shows community content."""
        click_blazor_tab(blazor_page, "Community Explorer")
        text = get_visible_text(blazor_page)
        assert "Community Reports" in text

    def test_blazor_tab_switch_back_to_search(self, blazor_page: Page):
        """Clicking Search tab after Community Explorer returns to search."""
        click_blazor_tab(blazor_page, "Community Explorer")
        click_blazor_tab(blazor_page, "Search")
        text = get_visible_text(blazor_page)
        assert "question" in text.lower() or "suggest" in text.lower()


# ---------------------------------------------------------------------------
# B) Community report selection behavior
# ---------------------------------------------------------------------------

class TestReportSelection:
    """Selecting a community report shows its details in both apps."""

    REPORT_TITLE = "Scrooge's Redemption Arc"
    REPORT_CONTENT_FRAGMENT = "Supernatural Intervention"

    def test_python_community_explorer_loads_data(self, python_page: Page):
        """Python community explorer loads the dataframe with reports."""
        click_python_tab(python_page, "Community Explorer")
        df_count = python_page.locator("[data-testid='stDataFrame']").count()
        assert df_count > 0, "Python community explorer missing dataframe"

    def test_blazor_report_selection_shows_details(self, blazor_page: Page):
        """Clicking a report in the Blazor app shows its full content."""
        click_blazor_tab(blazor_page, "Community Explorer")
        report_row = blazor_page.locator("tr", has_text=self.REPORT_TITLE)
        if report_row.count() > 0:
            report_row.first.click()
            blazor_page.wait_for_timeout(1500)
            text = get_visible_text(blazor_page)
            assert self.REPORT_CONTENT_FRAGMENT in text, \
                f"Report content '{self.REPORT_CONTENT_FRAGMENT}' not shown after click"
        else:
            text = get_visible_text(blazor_page)
            assert self.REPORT_TITLE in text

    def test_both_apps_have_community_explorer_structure(self, python_page: Page, blazor_page: Page):
        """Both apps have the Community Reports section with data."""
        click_python_tab(python_page, "Community Explorer")
        click_blazor_tab(blazor_page, "Community Explorer")

        py_text = get_visible_text(python_page)
        bl_text = get_visible_text(blazor_page)
        # Both have the Community Reports heading
        assert "Community Reports" in py_text
        assert "Community Reports" in bl_text
        # Both have the Selected Report panel
        assert "Selected Report" in py_text
        assert "Selected Report" in bl_text


# ---------------------------------------------------------------------------
# C) Community Explorer layout parity: two-panel split
# ---------------------------------------------------------------------------

class TestCommunityExplorerLayout:
    """Both apps use a two-panel layout for the Community Explorer."""

    def test_python_has_two_column_layout(self, python_page: Page):
        """Python Community Explorer has reports list and details panel."""
        click_python_tab(python_page, "Community Explorer")
        text = get_visible_text(python_page)
        assert "Community Reports" in text
        assert "Selected Report" in text

    def test_blazor_has_two_column_layout(self, blazor_page: Page):
        """Blazor Community Explorer has reports list and details panel."""
        click_blazor_tab(blazor_page, "Community Explorer")
        text = get_visible_text(blazor_page)
        assert "Community Reports" in text
        assert "Selected Report" in text


# ---------------------------------------------------------------------------
# D) Search input accepts text in both apps
# ---------------------------------------------------------------------------

class TestSearchInputBehavior:
    """Both apps accept text input in the search field."""

    TEST_QUERY = "What is the story about?"

    def test_python_search_input_accepts_text(self, python_page: Page):
        """The Python text input accepts typed text."""
        input_el = python_page.locator("input[type='text']").first
        if input_el.count() > 0:
            input_el.fill(self.TEST_QUERY)
            value = input_el.input_value()
            assert self.TEST_QUERY in value

    def test_blazor_search_input_accepts_text(self, blazor_page: Page):
        """The Blazor text input accepts typed text."""
        input_el = blazor_page.locator("#question-input")
        if input_el.count() > 0:
            input_el.fill(self.TEST_QUERY)
            value = input_el.input_value()
            assert self.TEST_QUERY in value


# ---------------------------------------------------------------------------
# E) Sidebar dataset selector works in both apps
# ---------------------------------------------------------------------------

class TestDatasetSelector:
    """Both apps have a dataset selector that shows the demo dataset."""

    def test_python_dataset_selector_has_christmas_carol(self, python_page: Page):
        """Python selectbox shows the Christmas Carol dataset."""
        sidebar = python_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text()
        assert "Christmas Carol" in sidebar_text or "christmas-carol" in sidebar_text.lower()

    def test_blazor_dataset_selector_has_christmas_carol(self, blazor_page: Page):
        """Blazor MudSelect shows the Christmas Carol dataset."""
        sidebar = blazor_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text()
        assert "Christmas Carol" in sidebar_text or "christmas-carol" in sidebar_text.lower()


# ---------------------------------------------------------------------------
# F) Semantic heading hierarchy parity
# ---------------------------------------------------------------------------

class TestSemanticHeadingHierarchy:
    """Both apps use equivalent heading hierarchies for content sections."""

    COMMUNITY_HEADINGS = ["Community Reports", "Selected Report"]

    def test_python_community_headings_order(self, python_page: Page):
        """Python Community Explorer headings appear in correct order."""
        click_python_tab(python_page, "Community Explorer")
        text = get_visible_text(python_page)
        pos_reports = text.find("Community Reports")
        pos_selected = text.find("Selected Report")
        assert pos_reports >= 0, "Missing 'Community Reports' heading"
        assert pos_selected >= 0, "Missing 'Selected Report' heading"
        assert pos_reports < pos_selected, \
            "'Community Reports' should appear before 'Selected Report'"

    def test_blazor_community_headings_order(self, blazor_page: Page):
        """Blazor Community Explorer headings appear in correct order."""
        click_blazor_tab(blazor_page, "Community Explorer")
        text = get_visible_text(blazor_page)
        pos_reports = text.find("Community Reports")
        pos_selected = text.find("Selected Report")
        assert pos_reports >= 0, "Missing 'Community Reports' heading"
        assert pos_selected >= 0, "Missing 'Selected Report' heading"
        assert pos_reports < pos_selected, \
            "'Community Reports' should appear before 'Selected Report'"

    def test_heading_order_matches_between_apps(
        self, python_page: Page, blazor_page: Page
    ):
        """Both apps present Community Explorer headings in the same order."""
        click_python_tab(python_page, "Community Explorer")
        click_blazor_tab(blazor_page, "Community Explorer")

        py_text = get_visible_text(python_page)
        bl_text = get_visible_text(blazor_page)

        for heading in self.COMMUNITY_HEADINGS:
            assert heading in py_text, f"Python missing '{heading}'"
            assert heading in bl_text, f"Blazor missing '{heading}'"

        py_order = [py_text.find(h) for h in self.COMMUNITY_HEADINGS]
        bl_order = [bl_text.find(h) for h in self.COMMUNITY_HEADINGS]
        assert py_order == sorted(py_order), "Python headings out of order"
        assert bl_order == sorted(bl_order), "Blazor headings out of order"


# ---------------------------------------------------------------------------
# G) Both apps have the same number of search toggle controls
# ---------------------------------------------------------------------------

class TestSearchToggleCount:
    """Both apps expose exactly 4 search type toggles."""

    TOGGLE_LABELS = [
        "Include basic RAG",
        "Include local search",
        "Include global search",
        "Include drift search",
    ]

    def test_python_has_four_toggles(self, python_page: Page):
        sidebar = python_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text().lower()
        count = sum(1 for t in self.TOGGLE_LABELS if t.lower() in sidebar_text)
        assert count == 4, f"Python has {count}/4 toggle labels"

    def test_blazor_has_four_toggles(self, blazor_page: Page):
        sidebar = blazor_page.locator("[data-testid='stSidebar']")
        sidebar_text = sidebar.inner_text().lower()
        count = sum(1 for t in self.TOGGLE_LABELS if t.lower() in sidebar_text)
        assert count == 4, f"Blazor has {count}/4 toggle labels"
