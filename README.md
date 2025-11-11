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

The fastest way to get started is to use the automated setup wizard:

1. Open the project with Unity 6.0 (6000.2.0f1 or newer).
2. Once scripts compile, select **Tools → Gateway → Create Starter Content** from the menu.
3. The wizard will create:
   - `Assets/GatewayData/States/ExampleFocusState.asset` – A sample visual state with material parameters
   - `Assets/GatewayData/Timelines/ExampleSessionTimeline.asset` – A timeline with one segment
   - `Assets/Scenes/GatewayDemo.unity` – A demo scene with configured components
   - `Assets/Materials/ExamplePulse.mat` – A basic material for testing
4. Create a URP Pipeline Asset:
   - Right-click in Project window → **Create → Rendering → URP Asset (with Universal Renderer)**
   - Assign it in **Edit → Project Settings → Graphics → Scriptable Render Pipeline Settings**
5. Assign an audio clip to the `AudioSource` component in the `GatewayDemo` scene.
6. Press Play to see the basic visual state system in action.

For a detailed checklist of all components and next steps, see `Docs/PROJECT_READINESS_CHECKLIST.md`.

## Getting started (Manual Setup)

If you prefer manual setup or need to create additional assets:

1. Open the project with Unity 6.0 (6000.0) or newer.
2. Import the Universal Render Pipeline (URP) and create a URP pipeline asset if you intend to use the visual setup described in the project plan.
3. Create Scriptable Objects via the **Assets → Create → Gateway** menu:
   - **Visual State** assets define material parameters and animation curves for each focus level.
   - **Session Timeline** assets assemble the ordered list of focus levels that the session will play through.
4. Add the `GatewayStateMachine` component to a scene GameObject that contains an `AudioSource` with the Gateway session audio clip.
5. Assign a `GatewayVisualController` to the state machine and populate its target materials list with the materials you want to drive.
6. (Optional) Add the `BreathingModelController` to the scene and assign a Sentis `ModelAsset`.  Feed preprocessed microphone data into the controller—or enable its automatic AudioSource capture—to modulate the visuals in real time.

## Local AI workflow


**Note**: This project uses `com.unity.sentis` (version 2.0.0) as the supported AI inference package. Unity 6 ships with Sentis 2.x, providing local on-device inference capabilities.
- Convert trained breathing or relaxation detection models to ONNX and place them in a Resources folder so they can be referenced by the Sentis `ModelAsset` field.
- Use the `BreathingModelController.onBreathMetric` UnityEvent to wire the AI model’s output to `GatewayVisualController.Tick` or to other scripts that react to the breathing intensity.
- Keep all inference on-device—do not integrate Unity’s cloud-based Assistant or Generator tools.

## Next steps

- Implement the cue detection system described in `gateway_visual_experience_project_plan.md` (e.g., using Unity Timeline or custom timestamp data).
- Build Shader Graph materials and VFX Graph systems that respond to the parameters driven by the visual controller.
- Extend the AI integration with additional local models (attention estimation, style modulation) while respecting the privacy-first philosophy.
