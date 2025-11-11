# AGENTS.md

## Project Overview
This repository contains a base Unity 6 project for building a Gateway Visual Experience powered by local AI models. It aims to transform the Gateway tape audio cues into dynamic, responsive visuals. The project uses Unity 6's Inference Engine (formerly Sentis) to run ONNX models locally on the user's CPU/GPU, ensuring offline operation.

## Setup commands
- Install Unity 6 with the required modules (e.g., standalone build support).
- Open the project using Unity Hub or from the command line:
  ```bash
  unity -projectPath .
  ```
- Ensure dependencies in `Packages/manifest.json` are installed. The project includes the `com.unity.ai.inference` package.

## Build and Run
- Use Unity's Build Settings to create a standalone build for your target platform (Windows, macOS or Linux).
- Scenes are located in `Assets/Scenes/`. Always add your new scenes to the build settings before building.

## Code Style
- Use C# conventions: PascalCase for classes and methods, camelCase for variables and private fields.
- Keep classes small and focused; use static helpers where appropriate.
- Prefer `ShaderGraph` or procedural mesh generation rather than importing large texture files.

- Document your scripts with XML comments and inline comments explaining the purpose and usage.

## Testing Instructions
- Use Unity's Test Framework for automated tests under a `Tests/` folder. Write edit-mode and play-mode tests where appropriate.
- For visual components, manually test by running the scene and verifying that AI-driven visuals respond to audio cues and input.

## AI & Inference
- All AI inference must run locally using the Inference Engine package (`com.unity.ai.inference`). Do not call external cloud services for generative content.
- Import ONNX models and run them via the Inference Engine; do not generate full images. The models should output parameters (e.g., color values, positions) that drive shader graphs or procedural generation.
- Keep visuals suggestive and abstract; respond to the user's audio or state rather than generating explicit images.

## Folder Structure Guidelines
- `Assets/` holds all game assets such as scenes, scripts, prefabs and art. Organize into subfolders like `Scenes/`, `Scripts/`, `Prefabs/`, `Art/`. If a directory would otherwise be empty, include a `.keep` file so it can be tracked in version control.
- `Packages/manifest.json` lists package dependencies. Do not leave the `dependencies` object empty; include required packages like `com.unity.ai.inference` so the Package Manager loads them.
- `ProjectSettings/ProjectVersion.txt` specifies the Unity version used. Keep it up to date so that other contributors open the project with the correct version.
- If you add new packages or scripts, update relevant documentation (e.g., `README.md`, `instructions_for_codex.md` and `AGENTS.md`) and the manifest.

## Development Guidelines
- **Local processing only**: All AI inference must run locally; avoid network connections during runtime.
- **Keep visuals abstract**: Use shader graphs and procedural geometry; avoid fixed textures. Visual cues should be suggestive and responsive rather than explicit.
- **Maintain clear organization**: Group related assets. Keep scripts modular and avoid clutter. Document what each folder or script does.
- **Document changes**: When adding new assets or scripts, include comments and update documentation to explain their purpose and usage.

## Pull Request Checklist
- Provide a clear description of your changes and the problem they solve.
- Ensure the project builds without errors.
- Update or add tests where applicable.
- Update documentation: `README.md`, `AGENTS.md`, and any relevant comments.
- Use descriptive commit messages following conventional commit style (e.g., `feat:`, `fix:`).

By following these instructions, AI coding agents like Codex can understand the context of this project, adhere to the design philosophy, and contribute effectively.
