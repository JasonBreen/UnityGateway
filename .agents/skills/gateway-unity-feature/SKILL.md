---
name: gateway-unity-feature
description: Implement or refactor scoped UnityGateway runtime features, including C# components, state-machine behavior, audio cue synchronization, ScriptableObjects, prefabs, and scenes. Use for Unity feature work in this repository; route model-inference details to gateway-local-inference and visual-effect design to gateway-procedural-visuals.
---

# Gateway Unity Feature

## Workflow

1. Read the root `AGENTS.md`, the affected code, and only the relevant section of `gateway_visual_experience_project_plan.md`.
2. Inspect `git status` and preserve unrelated work. Follow existing namespaces, serialization patterns, and folder boundaries.
3. Define the smallest runtime seam before editing: input, state, transition, output, and Unity lifecycle owner.
4. Keep MonoBehaviours focused. Put reusable configuration in ScriptableObjects and extract deterministic logic from Unity APIs when that makes tests practical.
5. Add XML documentation to public types and members. Add comments only where Unity lifecycle, threading, serialization, or ownership is non-obvious.
6. Preserve `.meta` files. Add new scenes to `ProjectSettings/EditorBuildSettings.asset` and avoid hand-editing complex serialized assets when Unity can author them safely.
7. Add Edit Mode, Play Mode, or .NET tests in proportion to the behavior. Prefer .NET tests only for logic that does not depend on Unity assemblies.
8. Update `README.md`, `instructions_for_codex.md`, or `AGENTS.md` only when setup, architecture, or contributor workflow changes.
9. Invoke `$gateway-validate` before handing off the change and report any Unity Editor validation that could not run.

## Guardrails

- Keep runtime processing offline and privacy-preserving.
- Keep visuals parameter-driven and abstract; do not add generated image pipelines or large fixed textures.
- Avoid broad package, scene, or asset rewrites for a narrow feature.
- Preserve inspector data when renaming serialized fields, using Unity migration attributes when required.
