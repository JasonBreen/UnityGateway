# UnityGateway

UnityGateway is a Unity 6 project that transforms the Monroe Institute's Gateway audio sessions into a reactive visual experience.
The repository provides a Sentis-first architecture so that every adaptive visual cue is driven by local inference rather than
cloud services.

## Repository layout

- `Assets/`
  - `Scripts/StateMachine/` – C# runtime components for sequencing focus levels and applying material parameters.
  - `Scripts/AI/` – Sentis integration scripts for running local neural networks that modulate the visuals.
  - `Scenes/Poolrooms.unity` – URP-ready showcase scene with modular poolroom prefabs, reflective water, and the Gateway controller stack.
  - `Prefabs/Poolrooms/` – Modular wall, floor, and water prefabs used to assemble the Poolrooms scene.
  - `Art/Materials/Poolrooms/` – URP Lit materials for the pool tile, wall segments, and translucent water plane.
  - `Art/VisualStates/` – Example `GatewayVisualState` and `GatewaySessionTimeline` assets that pair with the Poolrooms environment.
- `Packages/manifest.json` – Package manifest pinned to Unity Sentis 2.x and core Unity 6 modules.
- `Gateway.sln` and `DotNet/` – Lightweight .NET 8 solution used by CI to sanity-check the Unity project footprint.
- `gateway_visual_experience_project_plan.md` – Detailed product and technical design reference.
- `instructions_for_codex.md` – High-level guidelines for contributors and AI assistants.
- `.codex/agents/` – Project-scoped Codex specialists for Unity features, local inference, procedural visuals, and review.
- `.agents/skills/` – Reusable UnityGateway implementation and validation workflows shared by Codex agents.

## Codex workflows

Codex discovers the project agents and skills automatically when it runs inside this repository. Delegate focused work to `unity-feature-engineer`, `local-inference-engineer`, `procedural-visual-designer`, or `gateway-reviewer`. The corresponding skills can also be invoked directly as `$gateway-unity-feature`, `$gateway-local-inference`, `$gateway-procedural-visuals`, and `$gateway-validate`.

Run the repository-level validation workflow from PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\gateway-validate\scripts\validate.ps1
```

The baseline performs structural and offline-runtime checks, runs the .NET tests, and uses Unity batch mode when a `Unity` executable is available. Pass `-UnityPath` and `-RequireUnity` when Editor compilation must be mandatory.

## Getting started

1. Open the project with Unity 6.0 (6000.0) or newer.
2. Import the Universal Render Pipeline (URP) and create a URP pipeline asset if you intend to use the visual setup described in the project plan.
3. Create Scriptable Objects via the **Assets → Create → Gateway** menu:
   - **Visual State** assets define material parameters and animation curves for each focus level.
   - **Session Timeline** assets assemble the ordered list of focus levels that the session will play through.
4. Add the `GatewayStateMachine` component to a scene GameObject that contains an `AudioSource` with the Gateway session audio clip.
5. Assign a `GatewayVisualController` to the state machine and populate its target materials list with the materials you want to drive.
6. (Optional) Add the `BreathingModelController` to the scene and assign a Sentis `ModelAsset`.  Feed preprocessed microphone data into the controller—or enable its automatic AudioSource capture—to modulate the visuals in real time.  Unity 6 ships with Sentis 2.x, which this project is configured for.

## Local AI workflow

- Convert trained breathing or relaxation detection models to ONNX and place them in a Resources folder so they can be referenced by the Sentis `ModelAsset` field.
- Use the `BreathingModelController.onBreathMetric` UnityEvent to wire the AI model’s output to `GatewayVisualController.Tick` or to other scripts that react to the breathing intensity.
- Keep all inference on-device—do not integrate Unity’s cloud-based Assistant or Generator tools.

## Poolrooms demo assets

- The **Poolrooms** scene is authored for URP with simple Lit materials; no additional Shader Graphs or ONNX assets are required to run it.
- Sample `GatewayVisualState` and `GatewaySessionTimeline` assets live in `Assets/Art/VisualStates/` and drive the materials assigned to the `GatewayVisualController` in the scene.
- Modular floor, wall, and shallow water prefabs under `Assets/Prefabs/Poolrooms/` can be reassembled to prototype new layouts without duplicating geometry.

## Next steps

- Implement the cue detection system described in `gateway_visual_experience_project_plan.md` (e.g., using Unity Timeline or custom timestamp data).
- Build Shader Graph materials and VFX Graph systems that respond to the parameters driven by the visual controller.
- Extend the AI integration with additional local models (attention estimation, style modulation) while respecting the privacy-first philosophy.
