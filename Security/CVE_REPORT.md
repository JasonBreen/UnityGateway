# CVE Scan Summary

This repository currently relies on the Unity 6 package baseline described in [`Packages/manifest.json`](../Packages/manifest.json). To check the dependency list against the Open Source Vulnerability (OSV) database we added the utility script [`Tools/osv_scan.py`](../Tools/osv_scan.py).

## How the scan works

1. Parse the Unity manifest to enumerate every dependency.
2. Map dependencies that correspond to packages tracked by OSV (for now this only covers the bundled Newtonsoft.Json runtime published to NuGet).
3. Submit a query to `https://api.osv.dev/v1/query` for each supported package.
4. Write a machine-readable report to [`Security/osv_scan_results.json`](./osv_scan_results.json) alongside console output for quick status checks.

## Current results (UnityGateway)

Running the scanner in this environment produced the following output:

```text
Completed OSV scan attempt.
- com.unity.nuget.newtonsoft-json: unable to query OSV (Network error while querying OSV: Tunnel connection failed: 403 Forbidden)

The following packages do not have a supported OSV ecosystem mapping:
  - com.unity.modules.ai
  - com.unity.modules.animation
  - com.unity.modules.assetbundle
  - com.unity.modules.audio
  - com.unity.modules.cloth
  - com.unity.modules.director
  - com.unity.modules.imageconversion
  - com.unity.modules.imgui
  - com.unity.modules.jsonserialize
  - com.unity.modules.particlesystem
  - com.unity.modules.physics
  - com.unity.modules.physics2d
  - com.unity.modules.screencapture
  - com.unity.modules.terrain
  - com.unity.modules.terrainphysics
  - com.unity.modules.tilemap
  - com.unity.modules.ui
  - com.unity.modules.uielements
  - com.unity.modules.unityanalytics
  - com.unity.modules.unitywebrequest
  - com.unity.modules.unitywebrequestassetbundle
  - com.unity.modules.unitywebrequestaudio
  - com.unity.modules.unitywebrequesttexture
  - com.unity.modules.unitywebrequestwww
  - com.unity.modules.vehicles
  - com.unity.modules.video
  - com.unity.modules.vr
  - com.unity.modules.wind
  - com.unity.modules.xr
  - com.unity.sentis
  - com.unity.timeline
  - com.unity.visualeffectgraph

Detailed results written to Security/osv_scan_results.json
```

> **Note:** The outbound OSV query is blocked in the sandbox used for automated evaluation, which triggers the `403 Forbidden` tunnel error above. Running the script on a developer workstation with internet access should return vulnerability data for supported packages (primarily NuGet, npm, PyPI, etc.).

## Follow-up recommendations

- Run the scanner again from a machine with unrestricted internet access to retrieve live CVE data.
- Extend `PACKAGE_OVERRIDES` in [`Tools/osv_scan.py`](../Tools/osv_scan.py) as OSV gains support for Unity ecosystems or as we identify additional third-party packages that map to NuGet/npm identifiers.
- Consider supplementing OSV with the Unity Security Advisory board feed when Unity begins publishing machine-readable advisories for built-in modules.
