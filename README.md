# UnityGateway

UnityGateway is a Unity 6 project that transforms the Monroe Institute's Gateway audio sessions into a reactive visual experience.
The repository provides a Sentis-first architecture so that every adaptive visual cue is driven by local inference rather than
cloud services.

## Repository layout

- `Assets/`
  - `Scripts/StateMachine/` – C# runtime components for sequencing focus levels and applying material parameters.
  - `Scripts/AI/` – Sentis integration scripts for running local neural networks that modulate the visuals.
  - `Scenes/`, `Prefabs/`, `Art/` – Empty folders (tracked with `.keep` files) ready to hold Unity assets as development progresses.
- `Packages/manifest.json` – Package manifest pinned to Unity Sentis 2.x and core Unity 6 modules.
- `gateway_visual_experience_project_plan.md` – Detailed product and technical design reference.
- `instructions_for_codex.md` – High-level guidelines for contributors and AI assistants.

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

1. Open the project with Unity 6.0 (6000.0) or newer.
2. Create a URP pipeline asset via **Assets → Create → Rendering → URP Asset (with Universal Renderer)** and assign it in **Edit → Project Settings → Graphics → Scriptable Render Pipeline Settings**. The URP package (`com.unity.render-pipelines.universal` version 15.0.6) is already included in the project manifest.
3. Create Scriptable Objects via the **Assets → Create → Gateway** menu:
   - **Visual State** assets define material parameters and animation curves for each focus level.
   - **Session Timeline** assets assemble the ordered list of focus levels that the session will play through.
4. Add the `GatewayStateMachine` component to a scene GameObject that contains an `AudioSource` with the Gateway session audio clip.
5. Assign a `GatewayVisualController` to the state machine and populate its target materials list with the materials you want to drive.
6. (Optional) Add the `BreathingModelController` to the scene and assign a Sentis `ModelAsset`. Feed preprocessed microphone data into the controller—or enable its automatic AudioSource capture—to modulate the visuals in real time. This project uses `com.unity.sentis` version 2.0.0, which is the supported inference package for Unity 6.

## Local AI workflow

- Convert trained breathing or relaxation detection models to ONNX and place them in a Resources folder so they can be referenced by the Sentis `ModelAsset` field.
- Use the `BreathingModelController.onBreathMetric` UnityEvent to wire the AI model’s output to `GatewayVisualController.Tick` or to other scripts that react to the breathing intensity.
- Keep all inference on-device—do not integrate Unity’s cloud-based Assistant or Generator tools.

## Next steps

- Implement the cue detection system described in `gateway_visual_experience_project_plan.md` (e.g., using Unity Timeline or custom timestamp data).
- Build Shader Graph materials and VFX Graph systems that respond to the parameters driven by the visual controller.
- Extend the AI integration with additional local models (attention estimation, style modulation) while respecting the privacy-first philosophy.
