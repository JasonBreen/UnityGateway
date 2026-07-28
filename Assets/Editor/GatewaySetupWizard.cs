using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gateway.Visuals;

namespace Gateway.Editor
{
    /// <summary>
    /// Editor wizard that automates the creation of starter content for the Gateway visual experience.
    /// Invoked via Tools > Gateway > Create Starter Content.
    /// </summary>
    public static class GatewaySetupWizard
    {
        private const string MenuPath = "Tools/Gateway/Create Starter Content";
        private const int MenuPriority = 100;

        // Asset paths
        private const string StateAssetPath = "Assets/GatewayData/States/ExampleFocusState.asset";
        private const string TimelineAssetPath = "Assets/GatewayData/Timelines/ExampleSessionTimeline.asset";
        private const string ScenePath = "Assets/Scenes/GatewayDemo.unity";
        private const string MaterialPath = "Assets/Materials/ExamplePulse.mat";

        [MenuItem(MenuPath, false, MenuPriority)]
        public static void CreateStarterContent()
        {
            // Check if Unity has finished compiling scripts
            if (EditorApplication.isCompiling)
            {
                Debug.LogWarning("GatewaySetupWizard: Please wait for Unity to finish compiling scripts before running the wizard.");
                return;
            }

            // Check if required script types are available
            if (!IsTypeAvailable("Gateway.Visuals.GatewayVisualState") ||
                !IsTypeAvailable("Gateway.Visuals.GatewaySessionTimeline") ||
                !IsTypeAvailable("Gateway.Visuals.GatewayStateMachine") ||
                !IsTypeAvailable("Gateway.Visuals.GatewayVisualController"))
            {
                Debug.LogError("GatewaySetupWizard: Required Gateway scripts are missing or not compiled. Please ensure all scripts in Assets/Scripts are present and compiled.");
                return;
            }

            Debug.Log("GatewaySetupWizard: Starting creation of starter content...");

            // Create or focus existing assets
            var visualState = CreateOrFocusVisualState();
            var timeline = CreateOrFocusTimeline(visualState);
            var material = CreateOrFocusMaterial();
            CreateOrFocusScene(timeline, material);

            Debug.Log("GatewaySetupWizard: Starter content creation complete!");
            LogNextSteps();
        }

        private static bool IsTypeAvailable(string typeName)
        {
            return System.Type.GetType(typeName + ", Assembly-CSharp") != null;
        }

        private static GatewayVisualState CreateOrFocusVisualState()
        {
            // Check if asset already exists
            var existing = AssetDatabase.LoadAssetAtPath<GatewayVisualState>(StateAssetPath);
            if (existing != null)
            {
                Debug.Log($"GatewaySetupWizard: Visual state asset already exists at {StateAssetPath}. Focusing in Project window.");
                EditorGUIUtility.PingObject(existing);
                return existing;
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(StateAssetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create new visual state asset
            var state = ScriptableObject.CreateInstance<GatewayVisualState>();
            
            // Use reflection to set private fields since they don't have public setters
            var displayNameField = typeof(GatewayVisualState).GetField("displayName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            displayNameField?.SetValue(state, "Example Focus State");

            var materialParamsField = typeof(GatewayVisualState).GetField("materialParameters", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var materialParams = new System.Collections.Generic.List<MaterialParameter>();
            
            // Create a material parameter for the pulse speed
            var paramType = typeof(MaterialParameter);
            var param = System.Activator.CreateInstance(paramType);
            var propNameField = paramType.GetField("propertyName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var valueField = paramType.GetField("value", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            propNameField?.SetValue(param, "_PulseSpeed");
            valueField?.SetValue(param, 1.5f);
            materialParams.Add((MaterialParameter)param);
            
            materialParamsField?.SetValue(state, materialParams);

            AssetDatabase.CreateAsset(state, StateAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"GatewaySetupWizard: Created visual state asset at {StateAssetPath}");
            EditorGUIUtility.PingObject(state);

            return state;
        }

        private static GatewaySessionTimeline CreateOrFocusTimeline(GatewayVisualState visualState)
        {
            // Check if asset already exists
            var existing = AssetDatabase.LoadAssetAtPath<GatewaySessionTimeline>(TimelineAssetPath);
            if (existing != null)
            {
                Debug.Log($"GatewaySetupWizard: Session timeline asset already exists at {TimelineAssetPath}. Focusing in Project window.");
                EditorGUIUtility.PingObject(existing);
                return existing;
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(TimelineAssetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create new timeline asset
            var timeline = ScriptableObject.CreateInstance<GatewaySessionTimeline>();

            // Use reflection to set private fields
            var segmentsField = typeof(GatewaySessionTimeline).GetField("segments", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var segments = new System.Collections.Generic.List<TimelineSegment>();

            // Create a single segment referencing the example state
            var segmentType = typeof(TimelineSegment);
            var segment = System.Activator.CreateInstance(segmentType);
            var visualStateField = segmentType.GetField("visualState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var durationField = segmentType.GetField("durationSeconds", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            visualStateField?.SetValue(segment, visualState);
            durationField?.SetValue(segment, 60f);
            segments.Add((TimelineSegment)segment);

            segmentsField?.SetValue(timeline, segments);

            AssetDatabase.CreateAsset(timeline, TimelineAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"GatewaySetupWizard: Created session timeline asset at {TimelineAssetPath}");
            EditorGUIUtility.PingObject(timeline);

            return timeline;
        }

        private static Material CreateOrFocusMaterial()
        {
            // Check if asset already exists
            var existing = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (existing != null)
            {
                Debug.Log($"GatewaySetupWizard: Material already exists at {MaterialPath}. Focusing in Project window.");
                EditorGUIUtility.PingObject(existing);
                return existing;
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(MaterialPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a basic material
            // Note: This uses the default shader. Users should switch to a URP shader or Shader Graph.
            var material = new Material(Shader.Find("Standard"));
            material.SetFloat("_PulseSpeed", 1.5f);

            AssetDatabase.CreateAsset(material, MaterialPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"GatewaySetupWizard: Created material at {MaterialPath}. Note: You should switch this to a URP-compatible shader or Shader Graph.");
            EditorGUIUtility.PingObject(material);

            return material;
        }

        private static void CreateOrFocusScene(GatewaySessionTimeline timeline, Material material)
        {
            // Check if scene already exists
            if (File.Exists(ScenePath))
            {
                Debug.Log($"GatewaySetupWizard: Scene already exists at {ScenePath}. Opening scene.");
                EditorSceneManager.OpenScene(ScenePath);
                return;
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a new scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Create Gateway GameObject with required components
            var gatewayObject = new GameObject("Gateway System");
            var audioSource = gatewayObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;

            var stateMachine = gatewayObject.AddComponent<GatewayStateMachine>();
            
            // Use reflection to set private serialized fields
            var stateMachineType = typeof(GatewayStateMachine);
            var timelineField = stateMachineType.GetField("sessionTimeline", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timelineField?.SetValue(stateMachine, timeline);

            // Create visual controller GameObject
            var visualControllerObject = new GameObject("Visual Controller");
            var visualController = visualControllerObject.AddComponent<GatewayVisualController>();

            // Use reflection to set target materials
            var visualControllerType = typeof(GatewayVisualController);
            var materialsField = visualControllerType.GetField("targetMaterials", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var materials = new System.Collections.Generic.List<Material> { material };
            materialsField?.SetValue(visualController, materials);

            // Link visual controller to state machine
            var visualControllerField = stateMachineType.GetField("visualController", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            visualControllerField?.SetValue(stateMachine, visualController);

            // Save the scene
            EditorSceneManager.SaveScene(newScene, ScenePath);

            Debug.Log($"GatewaySetupWizard: Created and opened scene at {ScenePath}");
        }

        private static void LogNextSteps()
        {
            Debug.Log(@"
=== Next Steps ===

1. CREATE URP PIPELINE ASSET:
   - Go to Assets > Create > Rendering > URP Asset (with Universal Renderer)
   - Assign the created asset in Edit > Project Settings > Graphics > Scriptable Render Pipeline Settings

2. ASSIGN AUDIO CLIP:
   - Import a Gateway session audio file into your project
   - Select the Gateway System GameObject in the GatewayDemo scene
   - Assign the audio clip to the AudioSource component

3. UPDATE MATERIAL SHADER:
   - Select the ExamplePulse material in Assets/Materials/
   - Change the shader to a URP-compatible shader or create a custom Shader Graph
   - Ensure the shader has a _PulseSpeed float property (or update the visual state to match your shader's properties)

4. (OPTIONAL) ADD SENTIS MODEL:
   - Import an ONNX model file into your project
   - Create a ModelAsset from the ONNX file (drag onto the ModelAsset field in an inspector)
   - Add the BreathingModelController component to a GameObject in the scene
   - Assign the ModelAsset to the controller

5. PRESS PLAY:
   - Open the GatewayDemo scene
   - Press Play in Unity
   - The state machine should sequence through the timeline and update material parameters

For more information, see Docs/PROJECT_READINESS_CHECKLIST.md
            ");
        }
    }
}
