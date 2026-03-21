# Copyright (c) 2026 Microsoft Corporation.
# Licensed under the MIT License

from __future__ import annotations

import importlib.util
import sys
from pathlib import Path


def load_module():
    repo_root = Path(__file__).resolve().parents[2]
    script_path = repo_root / "scripts" / "benchmark_smoke.py"
    spec = importlib.util.spec_from_file_location("benchmark_smoke", script_path)
    assert spec is not None
    assert spec.loader is not None
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


def test_load_fixture_cases_uses_smoke_configs():
    module = load_module()
    repo_root = Path(__file__).resolve().parents[2]

    fixtures = module.load_fixture_cases(repo_root, ["text", "min-csv"])

    assert [fixture.name for fixture in fixtures] == ["min-csv", "text"]
    assert fixtures[0].queries[0].method == "local"
    assert fixtures[1].queries[-1].method == "basic"
    assert "community_reports.csv" in fixtures[1].expected_artifacts


def test_load_fixture_cases_skips_non_mapping_workflow_entries():
    module = load_module()
    repo_root = Path(__file__).resolve().parents[2]

    fixture = module.load_fixture_cases(repo_root, ["azure"])[0]

    assert fixture.name == "azure"
    assert fixture.expected_artifacts == ()
    assert fixture.uses_azure_input is True
    assert fixture.azure_input_container == "azurefixture"
    assert fixture.azure_input_base_dir == "input"


def test_render_markdown_report_includes_missing_output_notes():
    module = load_module()

    python_result = module.OperationResult(
        implementation="python",
        fixture="text",
        operation_type="index",
        operation_label="index:fast",
        method="fast",
        query=None,
        command=["uv", "run", "python", "-m", "graphrag", "index"],
        duration_seconds=12.5,
        exit_code=0,
        status="passed",
        notes=[],
        workflow_metrics={
            "extract_graph_nlp": {
                "overall_time_seconds": 5.0,
                "peak_memory_bytes": 100,
                "memory_delta_bytes": 10,
            }
        },
    )
    dotnet_result = module.OperationResult(
        implementation="dotnet",
        fixture="text",
        operation_type="index",
        operation_label="index:fast",
        method="fast",
        query=None,
        command=["dotnet", "GraphRag.dll", "index"],
        duration_seconds=1.25,
        exit_code=0,
        status="missing_outputs",
        notes=["Missing expected outputs: entities.csv"],
    )

    report = module.render_markdown_report([python_result], [dotnet_result])

    assert "Benchmark Comparison Report" in report
    assert "Missing expected outputs: entities.csv" in report
    assert "0.10" in report
    assert "extract_graph_nlp" in report


def test_render_markdown_report_explains_how_to_get_real_benchmark_run():
    module = load_module()

    dry_run_result = module.OperationResult(
        implementation="python",
        fixture="text",
        operation_type="index",
        operation_label="index:fast",
        method="fast",
        query=None,
        command=["uv", "run", "python", "-m", "graphrag", "index"],
        duration_seconds=0.0,
        exit_code=0,
        status="dry_run",
        stdout="[dry-run]",
    )

    report = module.render_markdown_report([dry_run_result], [])

    assert "This comparison used `--dry-run`" in report
    assert "OPENAI_API_KEY" in report
    assert "Run workflow" in report


def test_python_query_command_uses_cli_shape_from_fixture():
    module = load_module()
    repo_root = Path(__file__).resolve().parents[2]
    fixture = module.load_fixture_cases(repo_root, ["min-csv"])[0]

    command = module.python_cli_command(fixture, "query", repo_root, fixture.queries[0])

    assert command[:5] == ["uv", "run", "python", "-m", "graphrag"]
    assert command[5] == "query"
    assert "--method" in command
    assert "local" in command
