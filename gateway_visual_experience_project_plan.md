# Gateway Visual Experience Project Plan

## Introduction

The *Gateway Process* is a series of binaural audio sessions created by Robert Monroe’s Monroe Institute.  Each session (often called “Gateway tapes”) guides listeners through relaxation, hemispheric synchronization and states of expanded awareness.  Historically, the program has been audio‑only with suggested visualizations provided verbally by the guide.  Modern game‑engine technology allows us to provide a synchronized **visual scaffold** that evolves with the session and responds to the user’s internal state.  This document outlines how to build such a system in **Unity 6** while **keeping all AI models local** (no cloud‑dependency) and ensuring that the visuals remain suggestive rather than directive.

## Key Unity 6 features and AI capabilities

Unity 6.2 (released in 2025) introduces **Unity AI**, a suite of tools integrated into the Unity editor.  The release adds generative AI features for creating code, sprites, textures and animations and bundles them with a new inference engine【547134221950832†L23-L34】.  The generative tools allow artists to generate textures, materials, animations and sounds from text prompts【547134221950832†L52-L67】.  Unity notes that these tools rely on third‑party models such as LoRA‑fine‑tuned Stable Diffusion and Flux for sprites and custom models for textures【547134221950832†L56-L63】.  Unity AI also includes an **Assistant** tool that can automate routine tasks and answer technical questions using large‑language models (OpenAI’s GPT series and Meta’s Llama)【547134221950832†L70-L74】.

However, Unity AI’s generative tools and Assistant run **in the Unity Cloud**, meaning they require an internet connection and send prompt data to Unity’s servers【547134221950832†L104-L106】.  For this project we want to avoid cloud dependencies.  Fortunately, the third component of Unity AI—the **Inference Engine**—runs entirely on the user’s device and is designed for running custom models at runtime【547134221950832†L76-L80】.  This inference engine is essentially a rebranding of **Sentis**.

### Sentis and local model inference

**Sentis** is a neural‑network inference library shipped as a Unity package.  It allows you to import a trained model (ONNX or LiteRT/TensorFlow Lite format), prepare inputs and run inference directly on the target device’s CPU or GPU.  The library supports real‑time applications across all Unity‑supported platforms【110751609489431†L10-L17】.  Sentis is officially released and available via Unity’s Package Manager【110751609489431†L19-L20】.  It supports ONNX opsets 7–15 and LiteRT (TensorFlow Lite) models【110751609489431†L54-L62】.

Because Sentis runs locally, it is ideal for our project: we can deploy small AI models (e.g., a breathing detector or an abstract style‑mapping network) with no dependence on external servers.  Unity’s generative features and Assistant will **not** be used, ensuring privacy and offline operation.

## Overall architecture

### High‑level state machine

1. **Audio session parsing**: The Gateway session is divided into focus levels (e.g., Focus 10, Focus 12, Focus 15).  Each focus level is represented by a state in the visual state machine.  The state machine transitions when the audio file plays specific cues (e.g., “3 … 2 … 1 …  Focus Ten”).
2. **Time‑synchronized visuals**: For each state, a series of visual effects are defined.  The system loads these effects and fades between them in sync with the audio.
3. **AI‑driven parameter modulation**: Local AI models (running via Sentis) analyze microphone or biometric inputs and output parameters (breathing rate, heart rate or estimated relaxation level).  These parameters modulate shader properties to subtly adjust the visuals in real time.
4. **User control and UI**: A simple UI overlay allows the user to start, pause or end the session, adjust brightness/contrast and switch visual styles.

### Data flow

```
Audio playback  →  Cue detector  →  State machine  →  Visual controller →  Shaders/VFX
                      ↑                                        ↓
                 AI model (breath/relax detector via Sentis)  ← parameters from microphone
```

### Local AI model tasks

- **Breathing or relaxation detection**: A small convolutional neural network can be trained on spectrograms of breathing sounds to estimate breathing rate.  The model is exported to ONNX and loaded via Sentis.  As the user breathes, the model’s output modulates the amplitude and pacing of elements in the scene.
- **Attention/arousal estimation (optional)**: Additional models could infer whether the user is in a relaxed or heightened state from heart‑rate variability or galvanic skin response.  These models can also be loaded via Sentis if the required sensors are available.

## Implementation plan

### 1. Prepare Gateway session audio

1. Obtain the official Gateway session audio files.  Each file corresponds to a focus level and contains music, binaural beats and verbal guidance.
2. Create a script (e.g., in Python or using Unity’s *Timeline* package) that defines the exact timestamps of cues such as “Focus 10,” “Focus 12” and changes in guidance.  These timestamps will be used to trigger state transitions.

### 2. Set up Unity project

1. **Install Unity 6** and create a new project using the Universal Render Pipeline (URP).  URP provides good performance on a range of devices and supports sophisticated shader effects.
2. **Install Sentis via Package Manager**.  Verify that the package is installed by referencing the *com.unity.ai.inference* package in the manifest.
3. **Organize Scenes and resources**:
   - A single scene that acts as the container for the experience.
   - Scriptable Objects or data files representing each focus level’s visuals.
   - A manager component that reads the timeline and orchestrates state transitions.

### 3. Implement audio synchronization and state machine

1. Use Unity’s `AudioSource` to play the session audio.
2. Write a `GatewayStateMachine` script with states for Focus 3–10, Focus 12, Focus 15 and any additional extended focus levels.
3. Implement a timeline or event system that triggers state changes at the cue timestamps.  Unity’s `PlayableDirector` with a custom timeline track could be used to synchronize events to audio.

### 4. Design visual scenes

For each focus level, create unique but coherent visual environments:

- **Focus 3–10 (Body‑relaxation and breath‑sync)**: A dark background with subtle low‑frequency pulsations.  Use Shader Graph noise nodes to generate smooth gradient shifts.  Add fractal or perlin noise to create the sensation of a dim mist that expands and contracts.  Parameters like amplitude and speed will be modulated by breathing detection.
- **Focus 12 (Expanded awareness)**: Introduce geometric forms: slowly rotating tesseracts, recursive corridors or mandelbulb‑like shapes.  Use VFX Graph to spawn particles along curves and Shader Graph to distort space.  The colors should remain muted to encourage imagination.
- **Focus 15 (No‑time)**: Create an infinite horizon or void.  Use a volumetric fog with a distant subtle glow.  Place a few symbolic, slowly moving objects (e.g., orbs or floating stones).  Use camera dolly motion with near‑imperceptible parallax to create depth.  The background color can gradually shift towards deep blues and purples.

Ensure that each visual state is parameterized by a set of floats (e.g., `pulseRate`, `noiseIntensity`, `geometryRotation`).  These parameters can be modulated in code and by AI model outputs.

### 5. Integrate local AI models via Sentis

1. **Train or obtain a breathing detection model**: Use an existing open‑source model or train a small CNN on breathing and non‑breathing sounds.  Convert the trained model to ONNX.
2. **Load the model in Unity**: 
   - Use `ModelLoader.Load(path)` from the Sentis API to load the ONNX file.
   - Create an `IWorker` instance to run inference.
   - In `Update()`, capture audio from the microphone, process it into the input format expected by the model (e.g., compute a Mel spectrogram) and feed it to the model.
   - Retrieve the output (e.g., breath rate) and scale it to the desired parameter range.
3. **Apply the parameters to shaders**: Expose shader properties (via `Material.SetFloat`) and drive them with the model outputs.  For example, map breathing rate to the amplitude of noise displacement, or map relaxation probability to the camera’s depth of field.

### 6. Optional: Additional AI models

- **Voice‑to‑text**: If you want to allow voice commands (e.g., “pause session,” “change style”), load a local Whisper‑like model (converted to ONNX) via Sentis.
- **Emotion‑or style‑mapping networks**: A small style‑mapping network could modulate colors or shapes based on textual prompts (e.g., “cosmic,” “geometric”).

### 7. User interface and controls

1. Provide UI buttons to **start**, **pause**, **resume** and **end** the session.
2. Include a dropdown or toggle for **visual language** options:
   - **A**: Mystical / geometric / Pythagorean (tesseracts, Platonic solids, golden‑ratio spirals).
   - **B**: Biomechanical / cathedral‑dark (smooth metallic surfaces, Giger‑esque forms).
   - **C**: Cosmic / astral diffuse (nebulae, star fields).
   - **D**: Surreal / fever‑dream (unexpected shapes, saturated colors, glitch effects).
3. Allow users to adjust brightness, saturation and tempo sensitivity to suit personal preferences.

### 8. Testing and refinement

1. **Dry run** without AI model: Play a session with predetermined parameter curves to test the visual flow and ensure smooth transitions between states.
2. **Integrate AI**: Enable the Sentis model and test how the visuals respond to real breathing.  Adjust scaling factors to ensure that the response is perceptible but not distracting.
3. **Collect feedback**: Share the build with a small group of testers and gather feedback about comfort, immersion, and any dizziness or fatigue.  Refine the experience accordingly.

## Privacy and data considerations

- Unity AI’s generative services (Assistant and Generators) use cloud processing and may log prompt data【547134221950832†L104-L107】.  By relying exclusively on Sentis for local inference, we avoid sending user audio or biometric data to external servers.
- Ensure that any captured biometric data (microphone, heart‑rate sensor) is processed only in memory and not stored or transmitted.
- For legal compliance, include a privacy notice explaining that the experience uses local AI models only and does not transmit data off the device.

## Conclusion

By combining the cue structure of the Gateway tapes with Unity 6’s graphics capabilities and the **local inference** capabilities of Sentis, we can build a deeply immersive visual experience that enhances the original program without compromising privacy.  This approach leverages Unity’s modern rendering pipelines (URP/HDRP), Shader Graph and Visual Effects Graph, while respecting the user’s desire for offline operation.  The modular architecture allows for adding new focus levels, visual styles and AI inputs over time.  Most importantly, the visuals will serve as an **invitation to imagine**, not a distraction—reinforcing the core principle of the Gateway process.
