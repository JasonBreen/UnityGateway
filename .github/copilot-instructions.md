# GitHub Copilot Instructions for UnityGateway

## Project Overview
This repository contains a Unity 6 project for building a Gateway Visual Experience powered by local AI models. It transforms the Monroe Institute's Gateway audio sessions into dynamic, responsive visuals using Unity 6's Inference Engine (formerly Sentis) to run ONNX models locally on the user's CPU/GPU, ensuring offline operation.

The project provides a Sentis-first architecture where every adaptive visual cue is driven by local inference rather than cloud services, maintaining a privacy-first philosophy.

## Repository Structure

- `Assets/`
  - `Scripts/StateMachine/` – C# runtime components for sequencing focus levels and applying material parameters
  - `Scripts/AI/` – Sentis integration scripts for running local neural networks that modulate visuals
  - `Scenes/`, `Prefabs/`, `Art/` – Unity assets organized by type (use `.keep` files for empty tracked directories)
- `Packages/manifest.json` – Package manifest pinned to Unity Sentis 2.x and core Unity 6 modules; must include `com.unity.ai.inference`
- `ProjectSettings/ProjectVersion.txt` – Specifies the Unity version; keep up-to-date for version consistency
- `gateway_visual_experience_project_plan.md` – Detailed product and technical design reference
- `AGENTS.md` – Comprehensive guidelines for AI coding agents
- `instructions_for_codex.md` – High-level contributor guidelines

## Setup Commands

1. Install Unity 6.0 (6000.0) or newer with required modules (standalone build support)
2. Open the project using Unity Hub or command line:
   ```bash
   unity -projectPath .
   ```
3. Ensure dependencies in `Packages/manifest.json` are installed (especially `com.unity.ai.inference`)
4. Import Universal Render Pipeline (URP) and create a URP pipeline asset for the visual setup

## Build and Run

- Use Unity's Build Settings to create standalone builds for Windows, macOS, or Linux
- Scenes are located in `Assets/Scenes/` – always add new scenes to build settings before building
- Test builds by running the scene and verifying AI-driven visuals respond to audio cues

## Code Style Guidelines

- **C# Conventions**: PascalCase for classes and methods, camelCase for variables and private fields
- **Keep classes small and focused**: Use static helpers where appropriate
- **Prefer procedural generation**: Use ShaderGraph or procedural mesh generation over large texture files
- **Documentation**: Document scripts with XML comments and inline comments explaining purpose and usage
- **Conventional commits**: Use descriptive commit messages following conventional commit style (`feat:`, `fix:`, etc.)

## Testing Instructions

- Use Unity's Test Framework for automated tests under a `Tests/` folder
- Write edit-mode and play-mode tests where appropriate
- For visual components, manually test by running the scene and verifying that AI-driven visuals respond to audio cues and input
- Ensure the project builds without errors before submitting changes

## AI & Inference Requirements

**Critical: All AI inference must run locally using the Inference Engine package (`com.unity.ai.inference`)**

- Do NOT call external cloud services for generative content
- Import ONNX models and run them via the Inference Engine
- Models should output parameters (e.g., color values, positions) that drive shader graphs or procedural generation
- Do NOT generate full images directly
- Keep visuals suggestive and abstract; respond to user's audio or state
- Convert trained models (breathing, relaxation detection) to ONNX format
- Place models in Resources folder for Sentis `ModelAsset` field reference
- Use `BreathingModelController.onBreathMetric` UnityEvent to wire AI output to visual controllers
- Keep all inference on-device – no cloud-based Unity Assistant or Generator tools

## Development Guidelines

### Local Processing Only
- All AI inference must run locally
- Avoid network connections during runtime
- Maintain privacy-first philosophy

### Visual Design Philosophy
- Keep visuals abstract and suggestive, not explicit
- Use shader graphs and procedural geometry
- Avoid fixed textures
- Visual cues should be responsive to audio and user state

### Organization and Documentation
- Maintain clear folder hierarchy in `Assets/`
- Keep scripts modular and avoid clutter
- Group related assets together
- Document what each folder or script does
- Update `README.md`, `AGENTS.md`, and relevant comments when adding new features
- If a directory would be empty, include a `.keep` file for version control tracking

### Package Management
- Do not leave `dependencies` object in `Packages/manifest.json` empty
- Include required packages like `com.unity.ai.inference`
- Update documentation when adding new packages or scripts

## Pull Request Checklist

- [ ] Provide clear description of changes and problem solved
- [ ] Ensure project builds without errors
- [ ] Update or add tests where applicable
- [ ] Update documentation (`README.md`, `AGENTS.md`, and relevant comments)
- [ ] Use descriptive commit messages following conventional commit style
- [ ] Verify all changes maintain local-only AI processing
- [ ] Confirm visual changes maintain abstract/suggestive design philosophy

## Key Technical Decisions

1. **Unity 6 with Sentis 2.x**: The project is built on Unity 6 and uses the Inference Engine for local AI
2. **Privacy-First**: No cloud services, all processing happens on-device
3. **Audio-Driven Visuals**: The Gateway audio sessions drive the visual experience through state machines
4. **Scriptable Objects**: Use `Visual State` and `Session Timeline` assets to define material parameters and focus level sequences
5. **Component Architecture**: `GatewayStateMachine`, `GatewayVisualController`, and `BreathingModelController` are key components

## Important References

- Read `gateway_visual_experience_project_plan.md` for comprehensive project plan before implementing features
- Consult `AGENTS.md` for detailed guidelines on folder structure and development practices
- Review `README.md` for workflow examples and getting started information

## What to Avoid

- Do NOT integrate cloud-based AI services
- Do NOT generate explicit visual content
- Do NOT import large fixed texture files when procedural generation would work
- Do NOT leave directories or manifest dependencies empty
- Do NOT call external network services during runtime
- Do NOT modify or remove working code unnecessarily
