# Instructions for Codex

This repository contains a base Unity 6 project for building a Gateway Visual Experience powered by local AI models.  Please read and follow these guidelines when working with the project files.

## Folder Structure

- `Assets/` – Contains all game assets such as scenes, scripts, prefabs and art.  Additional subfolders such as `Scenes`, `Scripts`, `Prefabs`, and `Art` should be created as needed to keep content organized【147731013599812†L80-L107】.  Empty directories can be tracked in version control by placing a `.keep` file inside.
- `Packages/manifest.json` – Lists package dependencies for the project.  It should include Unity’s inference engine package (`com.unity.ai.inference`) to enable local neural network execution【535719397720415†L82-L112】.  Avoid leaving the dependencies object empty because the Package Manager won’t load any packages.
- `ProjectSettings/ProjectVersion.txt` – Specifies the exact Unity version used for this project.  Keep this file up‑to‑date so that editors open the project with the correct version.

## AI & Sentis / Inference Engine

The Gateway Visual Experience relies on local AI to adapt visuals in real time.  Use Unity’s Inference Engine package (`com.unity.ai.inference`, formerly Sentis) rather than cloud‑hosted APIs.  This package lets you import trained ONNX models and run them on the user’s device CPU/GPU【2153439978472†L74-L85】.  Your Unity scene scripts should call the inference API to compute shader parameters or other state variables instead of generating full images.

## Project Plan

The file `gateway_visual_experience_project_plan.md` in the repository root contains a comprehensive project plan explaining how the Gateway tapes are mapped to visual states, how to design the shaders, and how to integrate local AI models.  Read this document carefully before implementing any features.

## Development Guidelines

- **Local processing only:** All AI inference must run locally.  Do not call external services for generative content or rely on network connections during runtime.
- **Keep the visuals abstract:** Visual cues should be suggestive and responsive rather than explicit.  Use shader graphs and procedural geometry instead of fixed textures.
- **Maintain clear organization:** As the project grows, maintain an organized folder hierarchy.  Keep scripts modular, group related assets, and avoid clutter.
- **Document changes:** When adding new assets or scripts, include comments and readme files explaining their purpose and how to use them.

By following these instructions, you can help ensure that the Codex (or any other code assistant) understands the intent of the project and maintains compatibility with the overall design philosophy.
