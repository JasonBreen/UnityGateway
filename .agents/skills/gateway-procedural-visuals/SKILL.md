---
name: gateway-procedural-visuals
description: Design, implement, or review abstract responsive visuals for UnityGateway using procedural geometry, materials, Shader Graph, VFX Graph, and visual-state parameters. Use for focus-level environments, transitions, material mappings, comfort, and rendering-performance work.
---

# Gateway Procedural Visuals

## Workflow

1. Read the requested focus level or experience goal in `gateway_visual_experience_project_plan.md` and inspect the current scene, visual states, controller, materials, and prefabs.
2. Define a compact parameter contract before authoring assets: property name, range, neutral value, transition behavior, and source signal.
3. Build suggestive forms with procedural geometry, material properties, Shader Graph, or VFX Graph. Reuse modular assets and avoid large fixed textures.
4. Route changes through `GatewayVisualState` and `GatewayVisualController` or an equally explicit abstraction. Do not couple visual assets directly to inference workers.
5. Blend state transitions smoothly and keep a deterministic preview path that does not require a model or live microphone.
6. Prefer property blocks or deliberately instanced materials when per-object variation is needed. Avoid accidental mutation of shared project materials.
7. Budget particles, overdraw, transparent layers, shader complexity, and allocations for the target standalone platform.
8. Check comfort: avoid involuntary rapid flashes, harsh luminance jumps, excessive camera acceleration, and high-contrast strobing. Expose intensity controls when the effect can become fatiguing.
9. Verify the scene is in build settings, inspect the result in Play Mode, and invoke `$gateway-validate` for repository checks.

## Handoff

Report the visual parameters added, their valid ranges, the scene or prefab changed, the manual Play Mode checks performed, and any GPU-specific behavior still needing validation.
