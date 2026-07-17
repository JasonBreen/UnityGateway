---
name: gateway-local-inference
description: Build, debug, or review offline ONNX inference and audio or biometric preprocessing in UnityGateway using the repository's installed Unity Sentis or Inference Engine API. Use for ModelAsset loading, tensor contracts, worker backends, audio capture, smoothing, lifecycle, privacy, and performance work.
---

# Gateway Local Inference

## Workflow

1. Inspect `Packages/manifest.json`, the existing AI scripts, and the imported model contract before choosing APIs. Do not assume a package rename or version that the checkout does not use.
2. Write down the model contract: input and output names, shapes, sample rate, window size, channel layout, normalization, value range, and expected update frequency.
3. Separate capture, preprocessing, inference, smoothing, and visual mapping so each stage can be tested or replaced independently.
4. Keep audio-thread work bounded and allocation-free. Transfer data to the main thread through a thread-safe bounded buffer; never call general Unity APIs from the audio callback.
5. Prefer the appropriate GPU backend with a tested CPU fallback. Avoid blocking readbacks every frame, unnecessary tensor allocation, and unbounded queues.
6. Dispose workers, tensors, and model resources in the correct Unity lifecycle callbacks. Handle missing, malformed, or incompatible models with a clear disabled or deterministic fallback state.
7. Keep microphone and biometric samples in memory only. Add no network client, telemetry upload, or implicit recording path.
8. Provide a model-free dry-run or injected-sample path when feasible so behavior can be tested without distributing private audio or large model assets.
9. Add tests for preprocessing, normalization, smoothing, buffer bounds, and invalid model states where the API surface permits.
10. Invoke `$gateway-validate` and document the actual model and Unity runtime checks performed.

## Output Contract

- Emit small normalized parameters such as breath intensity, relaxation probability, or pacing.
- Clamp and smooth outputs before they drive visuals.
- Keep visual interpretation outside the model runner so `$gateway-procedural-visuals` can own the presentation mapping.
