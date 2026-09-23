# Project Readiness Checklist

This document captures the assessment items required to make the Gateway Visual Experience project ready for use with Unity 6.

## Overview

The Gateway project aims to provide a complete visual experience powered by local AI inference. This checklist ensures all necessary components are in place for a smooth development workflow.

## Essential Components

### ✅ 1. Scene Setup
- **Status**: Automated via Setup Wizard
- **Description**: A demo scene (`Assets/Scenes/GatewayDemo.unity`) with properly configured GameObjects
- **Requirements**:
  - GameObject with `GatewayStateMachine` component
  - GameObject with `GatewayVisualController` component
  - `AudioSource` component for Gateway session audio playback
  - Camera and lighting setup (default Unity scene objects)
- **How to Create**: Run `Tools > Gateway > Create Starter Content` menu item

### ✅ 2. Visual State Assets
- **Status**: Automated via Setup Wizard
- **Description**: ScriptableObject assets defining visual presentation for each focus level
- **Requirements**:
  - At least one `GatewayVisualState` asset in `Assets/GatewayData/States/`
  - Material parameters configured with shader property names and values
  - Optional animation parameters with curves
- **How to Create**: 
  - Automated: Run the Setup Wizard
  - Manual: Right-click in Project > Create > Gateway > Visual State

### ✅ 3. Session Timeline
- **Status**: Automated via Setup Wizard
- **Description**: ScriptableObject defining the chronological sequence of visual states
- **Requirements**:
  - At least one `GatewaySessionTimeline` asset in `Assets/GatewayData/Timelines/`
  - Timeline segments referencing Visual State assets
  - Duration and cue timestamp configuration for each segment
- **How to Create**:
  - Automated: Run the Setup Wizard
  - Manual: Right-click in Project > Create > Gateway > Session Timeline

### ⚠️ 4. URP Pipeline Asset
- **Status**: Manual creation required
- **Description**: Universal Render Pipeline asset for advanced rendering features
- **Requirements**:
  - URP Asset created via Create > Rendering > URP Asset (with Universal Renderer)
  - Pipeline asset assigned in Edit > Project Settings > Graphics
  - Compatible with Visual Effect Graph (already at version 15.0.6 in manifest)
- **Why Not Automated**: Asset serialization is Unity version-specific; manual creation ensures compatibility
- **How to Create**:
  1. Right-click in Project window
  2. Create > Rendering > URP Asset (with Universal Renderer)
  3. Open Edit > Project Settings > Graphics
  4. Assign the created asset to "Scriptable Render Pipeline Settings"

### ✅ 5. Materials
- **Status**: Basic material automated via Setup Wizard
- **Description**: Materials with shader properties that can be driven by the visual controller
- **Requirements**:
  - At least one material in `Assets/Materials/` folder
  - Material properties matching the Visual State configuration (e.g., `_PulseSpeed`)
  - Shader Graph or custom shader recommended for full feature support
- **How to Create**:
  - Automated: Run the Setup Wizard (creates basic Standard shader material)
  - Manual: Create custom ShaderGraph materials with desired properties

### ⚠️ 6. Audio Clip
- **Status**: Manual assignment required
- **Description**: Gateway session audio file
- **Requirements**:
  - Audio clip imported into the project
  - Clip assigned to the `AudioSource` component in the demo scene
- **Why Not Automated**: Audio clips are user-specific content
- **How to Assign**:
  1. Import your Gateway session audio file
  2. Open `Assets/Scenes/GatewayDemo.unity`
  3. Select the Gateway GameObject
  4. Assign the audio clip to the AudioSource component

### ⚠️ 7. Sentis Model Asset (Optional)
- **Status**: Manual creation required (optional feature)
- **Description**: ONNX model for AI-driven visual modulation
- **Requirements**:
  - ONNX model file imported into project (e.g., in Resources folder)
  - Sentis `ModelAsset` created and referencing the ONNX file
  - `BreathingModelController` component added to scene (optional)
- **Why Not Automated**: Models are user-specific and require training/acquisition
- **How to Create**:
  1. Import your ONNX model file
  2. Unity will automatically create a ModelAsset
  3. Add `BreathingModelController` component to a GameObject
  4. Assign the ModelAsset to the controller
  5. Wire the AI output events to the visual controller

### ⚠️ 8. Automated Tests (Future)
- **Status**: Planned for future PR
- **Description**: Unity Test Framework tests for runtime components
- **Requirements**:
  - Test framework package confirmed for Unity 6 compatibility
  - Edit-mode tests for ScriptableObject validation
  - Play-mode tests for state machine behavior
- **Why Not Implemented**: Test framework package version needs confirmation for Unity 6

### ✅ 9. Security Scan
- **Status**: Included in project workflow
- **Description**: Automated security scanning for vulnerabilities
- **Requirements**:
  - CodeQL security checks run on code changes
  - Dependency vulnerability scanning
- **Location**: See `Security/` folder for reports

## Package Dependencies

The following packages are required and configured in `Packages/manifest.json`:

- ✅ `com.unity.sentis` (2.0.0) - Local AI inference engine
- ✅ `com.unity.timeline` (1.8.7) - Timeline and sequencing support
- ✅ `com.unity.visualeffectgraph` (15.0.6) - VFX authoring
- ✅ `com.unity.render-pipelines.universal` (15.0.6) - URP rendering
- ✅ `com.unity.nuget.newtonsoft-json` (3.2.1) - JSON serialization
- ✅ Various Unity modules (AI, Animation, Audio, Physics, UI, etc.)

## Quick Start Workflow

1. **Run the Setup Wizard**
   - Open Unity 6 (version 6000.2.0f1 or newer)
   - Select `Tools > Gateway > Create Starter Content`
   - Wait for asset creation to complete

2. **Create URP Pipeline Asset**
   - Right-click in Project window
   - Create > Rendering > URP Asset (with Universal Renderer)
   - Assign in Edit > Project Settings > Graphics

3. **Assign Audio Clip**
   - Open `Assets/Scenes/GatewayDemo.unity`
   - Select Gateway GameObject
   - Assign your audio clip to the AudioSource

4. **Test the Demo**
   - Press Play in the Unity Editor
   - Verify audio playback and basic visual state transitions

5. **(Optional) Add AI Integration**
   - Import ONNX model
   - Add `BreathingModelController` component
   - Configure AI-driven visual modulation

## Development Priorities

### Phase 1: Core Setup (Current)
- ✅ Package manifest with all dependencies
- ✅ Automated setup wizard for starter content
- ✅ Example assets demonstrating the workflow
- ✅ Documentation and README updates

### Phase 2: Advanced Features (Future)
- ⚠️ Automated tests
- ⚠️ Additional example visual states and timelines
- ⚠️ Sample ShaderGraph materials
- ⚠️ Cue detection system integration
- ⚠️ Extended AI integration examples

### Phase 3: Optimization (Future)
- ⚠️ Performance profiling and optimization
- ⚠️ Build pipeline automation
- ⚠️ Cross-platform validation

## Troubleshooting

### Common Issues

**Issue**: Setup Wizard menu item not appearing
- **Solution**: Ensure scripts are compiled. Check for compilation errors in the Console.

**Issue**: Wizard reports missing Gateway scripts
- **Solution**: Verify all scripts in `Assets/Scripts/StateMachine/` and `Assets/Scripts/AI/` are present and compiling.

**Issue**: Material properties not animating
- **Solution**: Ensure your material's shader has the properties defined in the Visual State (e.g., `_PulseSpeed`). Use ShaderGraph for custom properties.

**Issue**: Audio not playing in scene
- **Solution**: Check that an audio clip is assigned to the AudioSource component and the AudioSource is not muted.

**Issue**: Scene doesn't appear in build
- **Solution**: Add `Assets/Scenes/GatewayDemo.unity` to Build Settings (File > Build Settings > Add Open Scenes).

## Additional Resources

- **Project Plan**: See `gateway_visual_experience_project_plan.md` for detailed technical design
- **Development Guidelines**: See `AGENTS.md` for coding conventions and AI integration rules
- **Contributor Instructions**: See `instructions_for_codex.md` for high-level guidelines
- **README**: See `README.md` for quick start and overview

## Status Legend

- ✅ **Automated/Complete**: Feature is implemented and automated via the Setup Wizard
- ⚠️ **Manual Required**: Requires user action (documented in this checklist)
- ❌ **Not Implemented**: Planned for future development

---

Last Updated: 2025-11-11
Unity Version: 6000.2.0f1
