#!/usr/bin/env python3
"""Attempt to scan Unity package dependencies for known CVEs using OSV."""
from __future__ import annotations

import json
import pathlib
import sys
from typing import Any, Dict, List, Optional

import urllib.error
import urllib.request

MANIFEST_PATH = pathlib.Path(__file__).resolve().parents[1] / "Packages" / "manifest.json"
RESULTS_PATH = pathlib.Path(__file__).resolve().parents[1] / "Security" / "osv_scan_results.json"
OSV_URL = "https://api.osv.dev/v1/query"

# Map Unity packages to OSV ecosystems / package names when known.
PACKAGE_OVERRIDES = {
    "com.unity.nuget.newtonsoft-json": {
        "ecosystem": "NuGet",
        "name": "Newtonsoft.Json",
    }
}


def load_manifest() -> Dict[str, str]:
    try:
        with MANIFEST_PATH.open("r", encoding="utf-8") as handle:
            manifest = json.load(handle)
    except FileNotFoundError as exc:
        raise SystemExit(f"Unable to open manifest at {MANIFEST_PATH}: {exc}") from exc

    dependencies = manifest.get("dependencies")
    if not isinstance(dependencies, dict):
        raise SystemExit("Manifest does not contain a 'dependencies' dictionary")

    return {str(name): str(version) for name, version in dependencies.items()}


def build_payload(package: str, version: str) -> Optional[Dict[str, Any]]:
    override = PACKAGE_OVERRIDES.get(package)
    if override:
        payload: Dict[str, Any] = {"package": {"name": override["name"], "ecosystem": override["ecosystem"]}}
        # When we only have a Unity wrapper version, stash it as metadata for reporting.
        payload["package_version"] = version
        return payload

    # OSV does not currently expose a dedicated Unity ecosystem. Return None so we can
    # skip the network request and surface that to the caller instead of sending
    # unsupported package identifiers to the API.
    return None


def query_osv(payload: Dict[str, Any]) -> Dict[str, Any]:
    body = json.dumps({k: v for k, v in payload.items() if k != "package_version"}).encode("utf-8")
    request = urllib.request.Request(OSV_URL, data=body, headers={"Content-Type": "application/json"})
    try:
        with urllib.request.urlopen(request, timeout=15) as response:  # noqa: S310 - urllib used intentionally
            data = json.load(response)
    except urllib.error.HTTPError as exc:  # pragma: no cover - exercised in CI only when API accessible
        return {
            "status": "error",
            "error": f"HTTP {exc.code} while querying OSV: {exc.reason}",
            "payload": payload,
        }
    except urllib.error.URLError as exc:
        return {
            "status": "error",
            "error": f"Network error while querying OSV: {exc.reason}",
            "payload": payload,
        }

    vulns = data.get("vulns") or []
    return {
        "status": "ok",
        "payload": payload,
        "vulnerabilities": vulns,
    }


def main() -> int:
    manifest_packages = load_manifest()
    results: List[Dict[str, Any]] = []
    unsupported: List[str] = []

    for package, version in sorted(manifest_packages.items()):
        payload = build_payload(package, version)
        if not payload:
            unsupported.append(package)
            continue

        outcome = query_osv(payload)
        outcome["unity_package"] = package
        outcome["unity_version"] = version
        results.append(outcome)

    RESULTS_PATH.write_text(json.dumps({"results": results, "unsupported": unsupported}, indent=2), encoding="utf-8")

    print("Completed OSV scan attempt.")
    if results:
        for entry in results:
            status = entry["status"]
            unity_package = entry["unity_package"]
            error = entry.get("error")
            if status != "ok":
                print(f"- {unity_package}: unable to query OSV ({error})")
            elif entry.get("vulnerabilities"):
                print(f"- {unity_package}: vulnerabilities found ({len(entry['vulnerabilities'])} entries)")
            else:
                print(f"- {unity_package}: no vulnerabilities reported by OSV")
    else:
        print("- No packages were eligible for OSV scanning.")

    if unsupported:
        print("\nThe following packages do not have a supported OSV ecosystem mapping:")
        for package in unsupported:
            print(f"  - {package}")

    print(f"\nDetailed results written to {RESULTS_PATH}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
