# Copyright (c) 2025 Microsoft Corporation.
# Licensed under the MIT License

"""
Pytest configuration and shared fixtures for UI parity tests.

Requirements:
  - Python container running on localhost:8501  (graphrag-search-python)
  - Blazor container running on localhost:8080  (graphrag-search-app)
  - Both containers seeded with the Christmas Carol demo dataset

Run:
  pytest tests/ui_parity/ -v
"""

import pytest
from playwright.sync_api import Page, sync_playwright

PYTHON_URL = "http://localhost:8501"
BLAZOR_URL = "http://localhost:8080"


@pytest.fixture(scope="session")
def browser_context():
    """Create a shared browser instance for all tests."""
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(viewport={"width": 1920, "height": 1080})
        yield context
        context.close()
        browser.close()


def _wait_for_streamlit_loaded(page: Page, timeout: int = 60000) -> None:
    """Wait for Streamlit app to finish initial load and render content."""
    page.wait_for_selector("[data-testid='stAppViewContainer']", timeout=timeout)
    page.wait_for_function(
        "() => document.body.innerText.includes('Suggest some questions')",
        timeout=timeout,
    )
    page.wait_for_timeout(2000)


@pytest.fixture()
def python_page(browser_context) -> Page:
    """Navigate to the Python Streamlit app and wait for it to load."""
    page = browser_context.new_page()
    page.goto(PYTHON_URL, timeout=60000)
    _wait_for_streamlit_loaded(page)
    yield page
    page.close()


@pytest.fixture()
def blazor_page(browser_context) -> Page:
    """Navigate to the Blazor app and wait for interactive circuit."""
    page = browser_context.new_page()
    page.goto(BLAZOR_URL, wait_until="networkidle", timeout=60000)
    page.wait_for_selector(".st-tabs", timeout=30000)
    # Wait for Blazor Server circuit to establish (WebSocket connected)
    page.wait_for_function(
        """() => {
            const scripts = document.querySelectorAll('script');
            for (const s of scripts) {
                if (s.src && s.src.includes('blazor')) return true;
            }
            return false;
        }""",
        timeout=15000,
    )
    # Give circuit time to connect and make DOM interactive
    page.wait_for_timeout(5000)
    yield page
    page.close()
