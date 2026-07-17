---
name: gateway-validate
description: Validate UnityGateway changes with repository structure checks, offline-runtime guardrails, .NET tests, diff inspection, and optional Unity batch-mode compilation. Use after implementation, during review, or when diagnosing whether the checkout is ready to build.
---

# Gateway Validate

## Run the Baseline

From the repository root, run:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\gateway-validate\scripts\validate.ps1
```

The script verifies required Unity project files, the local inference package, build-setting coverage for scenes, forbidden runtime networking APIs, and the .NET solution. It runs Unity batch-mode validation when `Unity` is on `PATH` or `-UnityPath` is supplied.

Use options only when the task requires them:

```powershell
# Require an explicit Unity Editor validation.
powershell -ExecutionPolicy Bypass -File .agents\skills\gateway-validate\scripts\validate.ps1 -UnityPath 'C:\Program Files\Unity\Hub\Editor\6000.2.0f1\Editor\Unity.exe' -RequireUnity

# Run structural checks without the .NET tests.
powershell -ExecutionPolicy Bypass -File .agents\skills\gateway-validate\scripts\validate.ps1 -SkipDotNet
```

## Review the Change

1. Inspect `git status --short --branch` and `git diff --check`.
2. Review the exact diff for behavior, serialized-field compatibility, Unity lifecycle ownership, thread safety, and missing `.meta` files.
3. Match validation depth to the change: Edit Mode tests for pure Unity logic, Play Mode tests for scene behavior, and a standalone build for rendering, audio, platform, or package changes.
4. Treat a missing Unity executable as an unverified Editor check, not as proof that the Unity project compiles.
5. Report commands, pass or fail results, skipped checks, and residual risk. Do not silently weaken a failing check.
