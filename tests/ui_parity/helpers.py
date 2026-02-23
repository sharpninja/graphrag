# Copyright (c) 2025 Microsoft Corporation.
# Licensed under the MIT License

"""Shared helpers for UI parity tests."""

from playwright.sync_api import Page


def get_visible_text(page: Page) -> str:
    """Return all visible text content on the page."""
    return page.evaluate("() => document.body.innerText")


def get_python_tabbar_frame(page: Page):
    """Return the iframe Frame object that holds the st_tabs.TabBar MUI tabs."""
    for frame in page.frames:
        if "TabBar" in frame.url:
            return frame
    return None


def click_python_tab(page: Page, tab_name: str) -> None:
    """Click a tab in the Python st_tabs.TabBar iframe by label text."""
    frame = get_python_tabbar_frame(page)
    if frame is None:
        raise RuntimeError("TabBar iframe not found")
    frame.locator(f"button[role='tab']:has-text('{tab_name}')").click()
    page.wait_for_timeout(3000)


def click_blazor_tab(page: Page, tab_name: str) -> None:
    """Click a tab in the Blazor app, retrying until the content appears."""
    for _ in range(3):
        page.locator(f"button[role='tab']:has-text('{tab_name}')").click()
        page.wait_for_timeout(3000)
        # Check if tab became active (content should have changed)
        active = page.locator(f"button[role='tab'].st-tab--active:has-text('{tab_name}')")
        if active.count() > 0:
            return
    # Final attempt
    page.locator(f"button[role='tab']:has-text('{tab_name}')").click()
    page.wait_for_timeout(3000)
