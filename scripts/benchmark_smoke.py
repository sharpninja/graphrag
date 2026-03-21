# Copyright (c) 2026 Microsoft Corporation.
# Licensed under the MIT License

"""Benchmark smoke indexing and query commands across implementations."""

from __future__ import annotations

import argparse
import asyncio
import json
import shutil
import subprocess  # noqa: S404
import time
from dataclasses import asdict, dataclass, field
from datetime import UTC, datetime
from pathlib import Path
from typing import Any

WELL_KNOWN_AZURITE_CONNECTION_STRING = (
    "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;"
    "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/"
    "K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1"
)


@dataclass(frozen=True)
class QueryCase:
    """Defines a single query benchmark case."""

    label: str
    method: str
    query: str
    community_level: int | None = None


@dataclass(frozen=True)
class FixtureCase:
    """Defines one smoke-test fixture benchmark case."""

    name: str
    root: Path
    input_type: str
    index_method: str
    expected_artifacts: tuple[str, ...]
    queries: tuple[QueryCase, ...]
    uses_azure_input: bool = False
    azure_input_container: str | None = None
    azure_input_base_dir: str | None = None


@dataclass
class OperationResult:
    """Captures the outcome of one benchmarked operation."""

    implementation: str
    fixture: str
    operation_type: str
    operation_label: str
    method: str
    query: str | None
    command: list[str]
    duration_seconds: float
    exit_code: int
    status: str
    expected_artifacts: list[str] = field(default_factory=list)
    produced_artifacts: list[str] = field(default_factory=list)
    notes: list[str] = field(default_factory=list)
    workflow_metrics: dict[str, dict[str, float | int | None]] = field(default_factory=dict)
    stdout: str = ""
    stderr: str = ""


def load_fixture_cases(repo_root: Path, fixture_names: list[str] | None = None) -> list[FixtureCase]:
    """Load benchmark fixtures from the smoke test configuration files."""
    fixtures_root = repo_root / "tests" / "fixtures"
    config_paths = sorted(fixtures_root.glob("*/config.json"))
    selected = set(fixture_names) if fixture_names else None
    cases: list[FixtureCase] = []

    for config_path in config_paths:
        fixture_name = config_path.parent.name
        if selected is not None and fixture_name not in selected:
            continue

        config = json.loads(config_path.read_text(encoding="utf-8"))
        workflow_config = config.get("workflow_config", {})
        azure_config = workflow_config.get("azure", {})
        queries = tuple(
            QueryCase(
                label=f"q{index + 1}",
                method=query_config["method"],
                query=query_config["query"],
                community_level=query_config.get("community_level"),
            )
            for index, query_config in enumerate(config.get("query_config", []))
        )
        expected_artifacts = tuple(
            artifact
            for workflow in workflow_config.values()
            if isinstance(workflow, dict)
            for artifact in workflow.get("expected_artifacts", [])
        )

        cases.append(
            FixtureCase(
                name=fixture_name,
                root=(repo_root / config["input_path"]).resolve(),
                input_type=config["input_type"],
                index_method=config["index_method"],
                expected_artifacts=expected_artifacts,
                queries=queries,
                uses_azure_input=bool(azure_config),
                azure_input_container=azure_config.get("input_container"),
                azure_input_base_dir=azure_config.get("input_base_dir"),
            )
        )

    return cases


def cleanup_fixture_outputs(fixture_root: Path) -> None:
    """Remove benchmark output and cache folders for a fixture."""
    shutil.rmtree(fixture_root / "output", ignore_errors=True)
    shutil.rmtree(fixture_root / "cache", ignore_errors=True)


async def prepare_azurite_input(case: FixtureCase) -> None:
    """Upload fixture input files into Azurite for Azure-backed smoke fixtures."""
    if not case.uses_azure_input or not case.azure_input_container:
        return

    from graphrag_storage.azure_blob_storage import AzureBlobStorage

    storage = AzureBlobStorage(
        connection_string=WELL_KNOWN_AZURITE_CONNECTION_STRING,
        container_name=case.azure_input_container,
    )
    storage._delete_container()  # noqa: SLF001
    storage._create_container()  # noqa: SLF001

    input_files = sorted((case.root / "input").glob("*.txt")) + sorted(
        (case.root / "input").glob("*.csv")
    )
    for input_file in input_files:
        relative_path = (
            str(Path(case.azure_input_base_dir) / input_file.name)
            if case.azure_input_base_dir
            else input_file.name
        )
        await storage.set(
            relative_path,
            input_file.read_text(encoding="utf-8"),
            encoding="utf-8",
        )


def cleanup_azurite_input(case: FixtureCase) -> None:
    """Delete any Azurite container created for the benchmark fixture."""
    if not case.uses_azure_input or not case.azure_input_container:
        return

    from graphrag_storage.azure_blob_storage import AzureBlobStorage

    storage = AzureBlobStorage(
        connection_string=WELL_KNOWN_AZURITE_CONNECTION_STRING,
        container_name=case.azure_input_container,
    )
    storage._delete_container()  # noqa: SLF001


def repo_relative(path: Path, repo_root: Path) -> str:
    """Return a repository-relative path using POSIX separators."""
    return path.resolve().relative_to(repo_root.resolve()).as_posix()


def python_cli_command(case: FixtureCase, operation: str, repo_root: Path, query: QueryCase | None = None) -> list[str]:
    """Build the Python CLI command for an index or query benchmark."""
    command = ["uv", "run", "python", "-m", "graphrag", operation]
    if operation == "index":
        command.extend(["--root", repo_relative(case.root, repo_root), "--method", case.index_method])
        return command

    if query is None:
        msg = "Query case is required for python query commands."
        raise ValueError(msg)

    command.extend(
        [
            query.query,
            "--root",
            repo_relative(case.root, repo_root),
            "--method",
            query.method,
        ]
    )
    if query.community_level is not None:
        command.extend(["--community-level", str(query.community_level)])
    return command


def dotnet_cli_command(case: FixtureCase, operation: str, repo_root: Path, query: QueryCase | None = None) -> list[str]:
    """Build the .NET CLI command for an index or query benchmark."""
    dll_candidates = [
        repo_root / "dotnet" / "src" / "GraphRag" / "bin" / "Release" / "net10.0" / "graphrag.dll",
        repo_root / "dotnet" / "src" / "GraphRag" / "bin" / "Release" / "net10.0" / "GraphRag.dll",
    ]
    dll_path = next((candidate for candidate in dll_candidates if candidate.exists()), dll_candidates[0])
    if dll_path.exists():
        command = ["dotnet", dll_path.as_posix(), operation]
    else:
        command = [
            "dotnet",
            "run",
            "--project",
            (repo_root / "dotnet" / "src" / "GraphRag" / "GraphRag.csproj").as_posix(),
            "--configuration",
            "Release",
            "--no-build",
            "--",
            operation,
        ]

    command.extend(["--root", repo_relative(case.root, repo_root)])
    if operation == "index":
        command.extend(["--method", case.index_method])
        return command

    if query is None:
        msg = "Query case is required for dotnet query commands."
        raise ValueError(msg)

    command.extend(["--method", query.method, "--query", query.query])
    return command


def run_command(command: list[str], cwd: Path, dry_run: bool) -> tuple[float, int, str, str]:
    """Execute one benchmark command and capture timing plus process output."""
    if dry_run:
        return 0.0, 0, "[dry-run]", ""

    started_at = time.perf_counter()
    completed = subprocess.run(  # noqa: S603
        command,
        cwd=cwd,
        capture_output=True,
        text=True,
        check=False,
    )
    duration = time.perf_counter() - started_at
    return duration, completed.returncode, completed.stdout, completed.stderr


def parse_python_workflow_metrics(output_root: Path) -> dict[str, dict[str, float | int | None]]:
    """Read Python per-workflow metrics from a smoke benchmark output folder."""
    stats_path = output_root / "stats.json"
    if not stats_path.exists():
        return {}

    stats = json.loads(stats_path.read_text(encoding="utf-8"))
    workflows = stats.get("workflows", {})
    return {
        workflow_name: {
            "overall_time_seconds": metrics.get("overall")
            or metrics.get("overall_time_seconds"),
            "peak_memory_bytes": metrics.get("peak_memory")
            or metrics.get("peak_memory_bytes"),
            "memory_delta_bytes": metrics.get("memory")
            or metrics.get("memory_delta_bytes"),
        }
        for workflow_name, metrics in workflows.items()
    }


def run_fixture_benchmark(
    implementation: str,
    case: FixtureCase,
    repo_root: Path,
    dry_run: bool,
) -> list[OperationResult]:
    """Run all configured benchmark operations for one fixture."""
    cleanup_fixture_outputs(case.root)
    if implementation == "python" and case.uses_azure_input and not dry_run:
        asyncio.run(prepare_azurite_input(case))

    output_root = case.root / "output"
    results: list[OperationResult] = []

    try:
        index_command = (
            python_cli_command(case, "index", repo_root)
            if implementation == "python"
            else dotnet_cli_command(case, "index", repo_root)
        )
        duration, exit_code, stdout, stderr = run_command(index_command, repo_root, dry_run)
        produced_artifacts = sorted(
            artifact
            for artifact in case.expected_artifacts
            if (output_root / artifact).exists()
        )
        missing_artifacts = sorted(set(case.expected_artifacts) - set(produced_artifacts))
        notes = []
        status = "passed" if exit_code == 0 else "failed"
        if missing_artifacts and not dry_run:
            status = "missing_outputs" if status == "passed" else status
            notes.append(
                "Missing expected outputs: " + ", ".join(missing_artifacts)
            )

        results.append(
            OperationResult(
                implementation=implementation,
                fixture=case.name,
                operation_type="index",
                operation_label=f"index:{case.index_method}",
                method=case.index_method,
                query=None,
                command=index_command,
                duration_seconds=duration,
                exit_code=exit_code,
                status="dry_run" if dry_run else status,
                expected_artifacts=list(case.expected_artifacts),
                produced_artifacts=produced_artifacts,
                notes=notes,
                workflow_metrics=parse_python_workflow_metrics(output_root)
                if implementation == "python"
                else {},
                stdout=stdout.strip(),
                stderr=stderr.strip(),
            )
        )

        if exit_code != 0:
            return results

        for query_case in case.queries:
            query_command = (
                python_cli_command(case, "query", repo_root, query_case)
                if implementation == "python"
                else dotnet_cli_command(case, "query", repo_root, query_case)
            )
            query_duration, query_exit_code, query_stdout, query_stderr = run_command(
                query_command, repo_root, dry_run
            )
            query_status = "passed" if query_exit_code == 0 else "failed"
            results.append(
                OperationResult(
                    implementation=implementation,
                    fixture=case.name,
                    operation_type="query",
                    operation_label=f"{query_case.label}:{query_case.method}",
                    method=query_case.method,
                    query=query_case.query,
                    command=query_command,
                    duration_seconds=query_duration,
                    exit_code=query_exit_code,
                    status="dry_run" if dry_run else query_status,
                    stdout=query_stdout.strip(),
                    stderr=query_stderr.strip(),
                )
            )
    finally:
        if implementation == "python" and case.uses_azure_input and not dry_run:
            cleanup_azurite_input(case)
        cleanup_fixture_outputs(case.root)

    return results


def summarize_results(results: list[OperationResult]) -> dict[str, int]:
    """Count benchmark operations by result status."""
    summary = {"passed": 0, "failed": 0, "missing_outputs": 0, "dry_run": 0}
    for result in results:
        summary[result.status] = summary.get(result.status, 0) + 1
    return summary


def render_markdown_report(
    python_results: list[OperationResult],
    dotnet_results: list[OperationResult],
) -> str:
    """Render a markdown comparison report for benchmark results."""
    generated_at = datetime.now(tz=UTC).isoformat()
    lines = [
        "# Benchmark Comparison Report",
        "",
        f"Generated at: `{generated_at}`",
        "",
        "## Status Summary",
        "",
        "| Implementation | Passed | Failed | Missing Outputs | Dry Run |",
        "| --- | ---: | ---: | ---: | ---: |",
    ]

    for implementation, results in (("python", python_results), ("dotnet", dotnet_results)):
        summary = summarize_results(results)
        lines.append(
            f"| {implementation} | {summary.get('passed', 0)} | "
            f"{summary.get('failed', 0)} | {summary.get('missing_outputs', 0)} | "
            f"{summary.get('dry_run', 0)} |"
        )

    lines.extend(
        [
            "",
            "## Apples-to-Apples Operation Comparison",
            "",
            "| Fixture | Operation | Query | Python (s) | Python Status | .NET (s) | .NET Status | Speed Ratio (.NET/Python) | Notes |",
            "| --- | --- | --- | ---: | --- | ---: | --- | ---: | --- |",
        ]
    )

    dotnet_map = {(result.fixture, result.operation_label): result for result in dotnet_results}
    for python_result in python_results:
        dotnet_result = dotnet_map.get((python_result.fixture, python_result.operation_label))
        dotnet_seconds = f"{dotnet_result.duration_seconds:.3f}" if dotnet_result else "n/a"
        dotnet_status = dotnet_result.status if dotnet_result else "missing"
        ratio = (
            f"{dotnet_result.duration_seconds / python_result.duration_seconds:.2f}"
            if dotnet_result and python_result.duration_seconds > 0
            else "n/a"
        )
        notes = list(python_result.notes)
        if dotnet_result is not None:
            notes.extend(dotnet_result.notes)
        combined_notes = sorted(set(notes))
        lines.append(
            "| "
            + " | ".join(
                [
                    python_result.fixture,
                    python_result.operation_label,
                    (python_result.query or "").replace("|", "\\|"),
                    f"{python_result.duration_seconds:.3f}",
                    python_result.status,
                    dotnet_seconds,
                    dotnet_status,
                    ratio,
                    "<br/>".join(combined_notes) if combined_notes else "",
                ]
            )
            + " |"
        )

    lines.extend(["", "## Python Workflow Metrics", ""])
    python_index_results = [
        result for result in python_results if result.operation_type == "index" and result.workflow_metrics
    ]
    if not python_index_results:
        lines.append("No Python workflow metrics were collected.")
    else:
        for result in python_index_results:
            lines.extend(
                [
                    f"### {result.fixture} ({result.method})",
                    "",
                    "| Workflow | Overall Time (s) | Peak Memory (bytes) | Memory Delta (bytes) |",
                    "| --- | ---: | ---: | ---: |",
                ]
            )
            for workflow_name, metrics in result.workflow_metrics.items():
                lines.append(
                    f"| {workflow_name} | {metrics.get('overall_time_seconds', 'n/a')} | "
                    f"{metrics.get('peak_memory_bytes', 'n/a')} | "
                    f"{metrics.get('memory_delta_bytes', 'n/a')} |"
                )
            lines.append("")

    return "\n".join(lines).rstrip() + "\n"


def serialize_results(results: list[OperationResult], repo_root: Path) -> dict[str, Any]:
    """Convert benchmark results into a JSON-serializable payload."""
    return {
        "generated_at": datetime.now(tz=UTC).isoformat(),
        "results": [asdict(result) for result in results],
        "repo_root": repo_root.as_posix(),
    }


def parse_results_file(path: Path) -> list[OperationResult]:
    """Load benchmark results from a JSON file."""
    payload = json.loads(path.read_text(encoding="utf-8"))
    return [OperationResult(**item) for item in payload["results"]]


def run_benchmarks(args: argparse.Namespace) -> int:
    """Run benchmarks for one implementation and write the JSON results."""
    repo_root = args.repo_root.resolve()
    cases = load_fixture_cases(repo_root, args.fixtures)
    results: list[OperationResult] = []
    for case in cases:
        results.extend(run_fixture_benchmark(args.implementation, case, repo_root, args.dry_run))

    args.output.write_text(
        json.dumps(serialize_results(results, repo_root), indent=2),
        encoding="utf-8",
    )
    return 0 if all(result.exit_code == 0 for result in results) else 1


def compare_benchmarks(args: argparse.Namespace) -> int:
    """Generate the markdown comparison report from two JSON result files."""
    python_results = parse_results_file(args.python_results)
    dotnet_results = parse_results_file(args.dotnet_results)
    args.output.write_text(
        render_markdown_report(python_results, dotnet_results),
        encoding="utf-8",
    )
    return 0


def build_parser() -> argparse.ArgumentParser:
    """Build the command-line parser for the benchmark helper."""
    parser = argparse.ArgumentParser(description="Run smoke benchmark comparisons across implementations.")
    subparsers = parser.add_subparsers(dest="command", required=True)

    run_parser = subparsers.add_parser("run", help="Run benchmark commands for one implementation.")
    run_parser.add_argument("--implementation", choices=["python", "dotnet"], required=True)
    run_parser.add_argument("--repo-root", type=Path, default=Path.cwd())
    run_parser.add_argument("--output", type=Path, required=True)
    run_parser.add_argument("--fixtures", nargs="*")
    run_parser.add_argument("--dry-run", action="store_true")
    run_parser.set_defaults(handler=run_benchmarks)

    compare_parser = subparsers.add_parser("compare", help="Generate a benchmark comparison report.")
    compare_parser.add_argument("--python-results", type=Path, required=True)
    compare_parser.add_argument("--dotnet-results", type=Path, required=True)
    compare_parser.add_argument("--output", type=Path, required=True)
    compare_parser.set_defaults(handler=compare_benchmarks)
    return parser


def main() -> int:
    """Run the benchmark helper CLI."""
    parser = build_parser()
    args = parser.parse_args()
    return args.handler(args)


if __name__ == "__main__":
    raise SystemExit(main())
