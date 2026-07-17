# Changelog

All notable changes to UnityGateway are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). The project does not yet publish numbered releases, so current work remains under `Unreleased`.

## [Unreleased]

### Added

- Add project-scoped Codex agents for Unity feature work, local inference, procedural visuals, and review.
- Add reusable Codex skills for implementation and repository validation.
- Add a PowerShell validation workflow covering Unity project structure, offline-runtime guardrails, .NET tests, and optional Unity batch-mode compilation.
- Add generated-directory exclusions for Unity, .NET, and IDE artifacts.

### Changed

- Upgrade the pinned Unity Editor from 6000.2.0f1 to 6000.5.4f1.
- Replace `com.unity.sentis` 2.0.0 with `com.unity.ai.inference` 2.4.1 and migrate the breathing model controller to `Unity.InferenceEngine`.
- Upgrade Timeline from 1.8.7 to 1.8.10.
- Upgrade Visual Effect Graph from 15.0.6 to the Unity 6.5-compatible 17.5.0 line.
- Refresh the repository README with the supported toolchain and current setup instructions.

### Fixed

- Correct the OSV scanner's Newtonsoft.Json mapping so it checks the embedded NuGet 13.0.2 version instead of returning advisories for every historical version.
