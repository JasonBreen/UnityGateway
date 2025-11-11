# Project Readiness Checklist

This document outlines the essential components and setup steps required to run the UnityGateway visual experience in Unity 6.

## Core Assets

### ✅ Scene Setup
- [ ] A Unity scene (e.g., `Assets/Scenes/GatewayDemo.unity`) containing:
  - [ ] A GameObject with the `GatewayStateMachine` component
  - [ ] An `AudioSource` component attached to the same GameObject (or referenced via the state machine's `audioSourceOverride` field)
  - [ ] A GameObject with the `GatewayVisualController` component
  - [ ] The state machine's `visualController` field assigned to the visual controller

### ✅ Visual State Assets
- [ ] At least one `GatewayVisualState` ScriptableObject asset created via **Assets → Create → Gateway → Visual State**
  - [ ] Configured with material parameters (e.g., `_PulseSpeed`, `_Color`, etc.)
  - [ ] Optional animation parameters defined for AI-driven modulation

### ✅ Session Timeline Asset
- [ ] A `GatewaySessionTimeline` ScriptableObject created via **Assets → Create → Gateway → Session Timeline**
  - [ ] Contains at least one timeline segment
  - [ ] Each segment references a `GatewayVisualState` asset
  - [ ] Segment durations configured appropriately

### ✅ AI Model Asset (Optional but Recommended)
- [ ] A Sentis `ModelAsset` imported from an ONNX model file
  - [ ] Model placed in a Resources folder or referenced directly in the inspector
  - [ ] `BreathingModelController` (or custom AI controller) added to the scene
  - [ ] Model asset assigned to the controller's `modelAsset` field

## Rendering Pipeline

### ✅ Universal Render Pipeline (URP)
- [ ] URP package installed (included in `Packages/manifest.json` as of this PR)
- [ ] A URP Renderer Asset created via **Assets → Create → Rendering → URP Asset (with Universal Renderer)**
  - [ ] Assigned in **Edit → Project Settings → Graphics → Scriptable Render Pipeline Settings**
- [ ] All materials updated to use URP shaders (e.g., `Universal Render Pipeline/Lit`, `Shader Graph`)

## Materials and Shaders

### ✅ Materials
- [ ] At least one material created (e.g., `Assets/Materials/ExamplePulse.mat`)
  - [ ] Material uses a URP-compatible shader or Shader Graph
  - [ ] Material defines properties that match the parameter names in your `GatewayVisualState` assets (e.g., `_PulseSpeed`)
  - [ ] Material assigned to the `GatewayVisualController`'s `targetMaterials` list

### ✅ Shader Graph (Optional)
- [ ] Custom Shader Graph created for procedural visuals
  - [ ] Exposes properties that can be driven by the visual controller
  - [ ] Supports dynamic parameter modulation from AI signals

## Audio

### ✅ Audio Clip
- [ ] A Gateway session audio clip imported into the project
  - [ ] Assigned to the `AudioSource` component's `clip` field
  - [ ] Audio clip format set appropriately (e.g., MP3, WAV, Vorbis)

## Optional Enhancements

### ⚠️ Tests
- [ ] Unity Test Framework package added (if not already present)
- [ ] Edit mode tests for scriptable object creation and validation
- [ ] Play mode tests for state machine sequencing

### ⚠️ Security
- [ ] CodeQL or security scan run on the codebase
- [ ] No hardcoded credentials or secrets in the repository
- [ ] All AI inference runs locally (no cloud service integrations)

## Quick Start

For users who want to skip manual setup, run the **Gateway Setup Wizard**:
1. In Unity, go to **Tools → Gateway → Create Starter Content**
2. The wizard will generate:
   - An example visual state asset
   - An example session timeline asset
   - A starter scene with wired-up components
   - A basic material with a `_PulseSpeed` property

After running the wizard:
- Create a URP Renderer Asset and assign it in **Project Settings → Graphics**
- Import a Gateway audio clip and assign it to the `AudioSource` in the scene
- (Optional) Import a Sentis model asset and assign it to the `BreathingModelController`

## Next Steps

Once the checklist is complete:
1. Open the `GatewayDemo` scene
2. Press Play in Unity
3. The audio should start, and the visual state machine should sequence through the timeline
4. Materials should update based on the parameters defined in each visual state
5. (If a Sentis model is connected) AI-driven modulation should respond to audio input or breathing signals

For detailed technical documentation, see:
- `gateway_visual_experience_project_plan.md` – Product and technical design
- `instructions_for_codex.md` – Contribution guidelines
- `AGENTS.md` – AI agent instructions
