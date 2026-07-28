# UnityGateway

UnityGateway is a Unity 6.5 project that transforms the Monroe Institute's Gateway audio sessions into a reactive visual experience.
The repository provides an Inference Engine-first architecture so that every adaptive visual cue is driven by local inference rather than
cloud services.

## Supported toolchain

| Component | Pinned version |
| --- | --- |
| Unity Editor | 6000.5.4f1 |
| Unity Inference Engine | 2.4.1 |
| Timeline | 1.8.10 |
| Visual Effect Graph | 17.5.0 |
| .NET SDK | 8.0 or newer |

Install Unity 6000.5.4f1 through Unity Hub with the standalone build-support module for your target platform. Opening the project with an older editor is unsupported because Unity serializes project and package metadata against the pinned editor line.

## Repository layout

- `Assets/`
  - `Scripts/StateMachine/` – C# runtime components for sequencing focus levels and applying material parameters.
  - `Scripts/AI/` – Sentis integration scripts for running local neural networks that modulate the visuals.
  - `Scenes/Poolrooms.unity` – URP-ready showcase scene with modular poolroom prefabs, reflective water, and the Gateway controller stack.
  - `Prefabs/Poolrooms/` – Modular wall, floor, and water prefabs used to assemble the Poolrooms scene.
  - `Art/Materials/Poolrooms/` – URP Lit materials for the pool tile, wall segments, and translucent water plane.
  - `Art/VisualStates/` – Example `GatewayVisualState` and `GatewaySessionTimeline` assets that pair with the Poolrooms environment.
- `Packages/manifest.json` – Package manifest pinned to Inference Engine 2.4.1 and Unity 6.5-compatible packages.
- `Gateway.sln` and `DotNet/` – Lightweight .NET 8 solution used by CI to sanity-check the Unity project footprint.
- `gateway_visual_experience_project_plan.md` – Detailed product and technical design reference.
- `instructions_for_codex.md` – High-level guidelines for contributors and AI assistants.
- `CHANGELOG.md` – Notable project, toolchain, and workflow changes.
- `.codex/agents/` – Project-scoped Codex specialists for Unity features, local inference, procedural visuals, and review.
- `.agents/skills/` – Reusable UnityGateway implementation and validation workflows shared by Codex agents.

## Codex workflows

Codex discovers the project agents and skills automatically when it runs inside this repository. Delegate focused work to `unity-feature-engineer`, `local-inference-engineer`, `procedural-visual-designer`, or `gateway-reviewer`. The corresponding skills can also be invoked directly as `$gateway-unity-feature`, `$gateway-local-inference`, `$gateway-procedural-visuals`, and `$gateway-validate`.

Run the repository-level validation workflow from PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File .agents\skills\gateway-validate\scripts\validate.ps1
```

The baseline performs structural and offline-runtime checks, runs the .NET tests, and uses Unity batch mode when a `Unity` executable is available. Pass `-UnityPath` and `-RequireUnity` when Editor compilation must be mandatory.

## Quick Start (Auto Setup)

The fastest way to get started is to use the **Gateway Setup Wizard**, which automates the creation of all required starter assets:

1. Open the project with Unity 6.0 (6000.0) or newer (specifically tested with 6000.2.0f1).
2. In Unity's menu bar, go to **Tools → Gateway → Create Starter Content**.
3. The wizard will generate:
   - An example visual state asset (`Assets/GatewayData/States/ExampleFocusState.asset`)
   - An example session timeline asset (`Assets/GatewayData/Timelines/ExampleSessionTimeline.asset`)
   - A starter scene (`Assets/Scenes/GatewayDemo.unity`) with wired-up components
   - A basic material (`Assets/Materials/ExamplePulse.mat`) with a `_PulseSpeed` property

After running the wizard, complete these additional steps:

4. **Create a URP Pipeline Asset**: Go to **Assets → Create → Rendering → URP Asset (with Universal Renderer)**, then assign it in **Edit → Project Settings → Graphics → Scriptable Render Pipeline Settings**.
5. **Assign an audio clip**: Import a Gateway session audio file and assign it to the `AudioSource` component on the "Gateway System" GameObject in the `GatewayDemo` scene.
6. **Update the material shader**: Select `ExamplePulse.mat` and change it to a URP-compatible shader or Shader Graph. Ensure the shader exposes a `_PulseSpeed` float property (or update the visual state to match your shader's properties).
7. **(Optional) Add a Sentis model**: Import an ONNX model, create a `ModelAsset`, add the `BreathingModelController` component to the scene, and assign the model asset.

For a detailed checklist of all required components, see [`Docs/PROJECT_READINESS_CHECKLIST.md`](Docs/PROJECT_READINESS_CHECKLIST.md).

## Getting started (Manual Setup)

If you prefer to set up the project manually instead of using the wizard:

1. Install and open the project with Unity 6000.5.4f1. Unity Hub reads the exact version from `ProjectSettings/ProjectVersion.txt`.
2. Import the Universal Render Pipeline (URP) and create a URP pipeline asset if you intend to use the visual setup described in the project plan. The URP package (`com.unity.render-pipelines.universal` 17.5.0) is already included in `Packages/manifest.json`.
3. Create Scriptable Objects via the **Assets → Create → Gateway** menu:
   - **Visual State** assets define material parameters and animation curves for each focus level.
   - **Session Timeline** assets assemble the ordered list of focus levels that the session will play through.
4. Add the `GatewayStateMachine` component to a scene GameObject that contains an `AudioSource` with the Gateway session audio clip.
5. Assign a `GatewayVisualController` to the state machine and populate its target materials list with the materials you want to drive.
6. (Optional) Add the `BreathingModelController` to the scene and assign an Inference Engine `ModelAsset`. Feed preprocessed microphone data into the controller—or enable its automatic AudioSource capture—to modulate the visuals in real time. The project uses `com.unity.ai.inference` 2.4.1 and the `Unity.InferenceEngine` API.

## Local AI workflow

- Convert trained breathing or relaxation detection models to ONNX and place them in a Resources folder so they can be referenced by the Sentis `ModelAsset` field.
- Use the `BreathingModelController.OnBreathMetric` UnityEvent to wire the AI model’s output to `GatewayVisualController.Tick` or to other scripts that react to the breathing intensity.
- Keep all inference on-device—do not integrate Unity’s cloud-based Assistant or Generator tools.

## Poolrooms demo assets

- The **Poolrooms** scene is authored for URP with simple Lit materials; no additional Shader Graphs or ONNX assets are required to run it.
- Sample `GatewayVisualState` and `GatewaySessionTimeline` assets live in `Assets/Art/VisualStates/` and drive the materials assigned to the `GatewayVisualController` in the scene.
- Modular floor, wall, and shallow water prefabs under `Assets/Prefabs/Poolrooms/` can be reassembled to prototype new layouts without duplicating geometry.

## Next steps

- Implement the cue detection system described in `gateway_visual_experience_project_plan.md` (e.g., using Unity Timeline or custom timestamp data).
- Build Shader Graph materials and VFX Graph systems that respond to the parameters driven by the visual controller.
- Extend the AI integration with additional local models (attention estimation, style modulation) while respecting the privacy-first philosophy.
