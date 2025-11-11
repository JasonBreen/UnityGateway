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

## Next steps

- Implement the cue detection system described in `gateway_visual_experience_project_plan.md` (e.g., using Unity Timeline or custom timestamp data).
- Build Shader Graph materials and VFX Graph systems that respond to the parameters driven by the visual controller.
- Extend the AI integration with additional local models (attention estimation, style modulation) while respecting the privacy-first philosophy.
