using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gateway.Visuals;

namespace Gateway.Editor
{
    /// <summary>
    /// Editor wizard that automates the creation of starter content for Gateway Visual Experience.
    /// Menu: Tools > Gateway > Create Starter Content
    /// </summary>
    public static class GatewaySetupWizard
    {
        private const string MenuPath = "Tools/Gateway/Create Starter Content";
        private const string GatewayDataPath = "Assets/GatewayData";
        private const string StatesPath = "Assets/GatewayData/States";
        private const string TimelinesPath = "Assets/GatewayData/Timelines";
        private const string MaterialsPath = "Assets/Materials";
        private const string ScenesPath = "Assets/Scenes";
        
        private const string ExampleStateName = "ExampleFocusState";
        private const string ExampleTimelineName = "ExampleSessionTimeline";
        private const string ExampleMaterialName = "ExamplePulse";
        private const string ExampleSceneName = "GatewayDemo";

        [MenuItem(MenuPath, priority = 0)]
        public static void CreateStarterContent()
        {
            // Check if scripts are compiled and ready
            if (EditorApplication.isCompiling)
            {
                Debug.LogWarning("[Gateway Setup] Please wait for script compilation to complete before running the wizard.");
                return;
            }

            // Verify required types are available
            if (!VerifyRequiredTypes())
            {
                Debug.LogError("[Gateway Setup] Required Gateway scripts are missing. Please ensure all scripts in Assets/Scripts are compiled successfully.");
                return;
            }

            Debug.Log("[Gateway Setup] Starting creation of starter content...");

            // Create directory structure
            CreateDirectories();

            // Create assets
            bool stateCreated = CreateExampleVisualState();
            bool timelineCreated = CreateExampleTimeline();
            bool materialCreated = CreateExampleMaterial();
            bool sceneCreated = CreateExampleScene();

            // Save all assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Log completion message with next steps
            LogCompletionMessage(stateCreated, timelineCreated, materialCreated, sceneCreated);
        }

        private static bool VerifyRequiredTypes()
        {
            // Check if the key Gateway types are available
            var stateType = typeof(GatewayVisualState);
            var timelineType = typeof(GatewaySessionTimeline);
            var stateMachineType = typeof(GatewayStateMachine);
            var controllerType = typeof(GatewayVisualController);

            return stateType != null && timelineType != null && stateMachineType != null && controllerType != null;
        }

        private static void CreateDirectories()
        {
            EnsureDirectoryExists(GatewayDataPath);
            EnsureDirectoryExists(StatesPath);
            EnsureDirectoryExists(TimelinesPath);
            EnsureDirectoryExists(MaterialsPath);
            EnsureDirectoryExists(ScenesPath);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentPath = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                
                // Ensure parent exists first
                if (!string.IsNullOrEmpty(parentPath) && parentPath != "Assets" && !AssetDatabase.IsValidFolder(parentPath))
                {
                    EnsureDirectoryExists(parentPath);
                }
                
                AssetDatabase.CreateFolder(parentPath, folderName);
                Debug.Log($"[Gateway Setup] Created folder: {path}");
            }
        }

        private static bool CreateExampleVisualState()
        {
            string assetPath = $"{StatesPath}/{ExampleStateName}.asset";
            
            // Check if asset already exists
            var existingAsset = AssetDatabase.LoadAssetAtPath<GatewayVisualState>(assetPath);
            if (existingAsset != null)
            {
                Debug.Log($"[Gateway Setup] Visual State already exists at {assetPath}. Skipping creation.");
                EditorGUIUtility.PingObject(existingAsset);
                Selection.activeObject = existingAsset;
                return false;
            }

            // Create new visual state
            var visualState = ScriptableObject.CreateInstance<GatewayVisualState>();
            
            // Use reflection to set private fields since they don't have public setters
            var displayNameField = typeof(GatewayVisualState).GetField("displayName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (displayNameField != null)
            {
                displayNameField.SetValue(visualState, "Example Focus State");
            }

            // Add an example material parameter
            var materialParametersField = typeof(GatewayVisualState).GetField("materialParameters",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (materialParametersField != null)
            {
                var parametersList = new System.Collections.Generic.List<MaterialParameter>();
                var parameter = new MaterialParameter();
                
                var paramPropertyNameField = typeof(MaterialParameter).GetField("propertyName",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var paramValueField = typeof(MaterialParameter).GetField("value",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (paramPropertyNameField != null && paramValueField != null)
                {
                    paramPropertyNameField.SetValue(parameter, "_PulseSpeed");
                    paramValueField.SetValue(parameter, 1.0f);
                    parametersList.Add(parameter);
                }
                
                materialParametersField.SetValue(visualState, parametersList);
            }

            AssetDatabase.CreateAsset(visualState, assetPath);
            Debug.Log($"[Gateway Setup] Created Visual State at {assetPath}");
            EditorGUIUtility.PingObject(visualState);
            Selection.activeObject = visualState;
            return true;
        }

        private static bool CreateExampleTimeline()
        {
            string assetPath = $"{TimelinesPath}/{ExampleTimelineName}.asset";
            
            // Check if asset already exists
            var existingAsset = AssetDatabase.LoadAssetAtPath<GatewaySessionTimeline>(assetPath);
            if (existingAsset != null)
            {
                Debug.Log($"[Gateway Setup] Session Timeline already exists at {assetPath}. Skipping creation.");
                EditorGUIUtility.PingObject(existingAsset);
                return false;
            }

            // Load the visual state we just created (or existing one)
            string stateAssetPath = $"{StatesPath}/{ExampleStateName}.asset";
            var visualState = AssetDatabase.LoadAssetAtPath<GatewayVisualState>(stateAssetPath);

            // Create new timeline
            var timeline = ScriptableObject.CreateInstance<GatewaySessionTimeline>();
            
            // Use reflection to add a segment
            var segmentsField = typeof(GatewaySessionTimeline).GetField("segments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (segmentsField != null && visualState != null)
            {
                var segmentsList = new System.Collections.Generic.List<TimelineSegment>();
                var segment = new TimelineSegment();
                
                var visualStateField = typeof(TimelineSegment).GetField("visualState",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var durationField = typeof(TimelineSegment).GetField("durationSeconds",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (visualStateField != null && durationField != null)
                {
                    visualStateField.SetValue(segment, visualState);
                    durationField.SetValue(segment, 60.0f);
                    segmentsList.Add(segment);
                }
                
                segmentsField.SetValue(timeline, segmentsList);
            }

            AssetDatabase.CreateAsset(timeline, assetPath);
            Debug.Log($"[Gateway Setup] Created Session Timeline at {assetPath}");
            EditorGUIUtility.PingObject(timeline);
            return true;
        }

        private static bool CreateExampleMaterial()
        {
            string assetPath = $"{MaterialsPath}/{ExampleMaterialName}.mat";
            
            // Check if material already exists
            var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (existingMaterial != null)
            {
                Debug.Log($"[Gateway Setup] Material already exists at {assetPath}. Skipping creation.");
                EditorGUIUtility.PingObject(existingMaterial);
                return false;
            }

            // Create a basic material with Standard shader
            var material = new Material(Shader.Find("Standard"));
            material.name = ExampleMaterialName;
            
            // Set a property that matches our example state (_PulseSpeed)
            // Note: Standard shader doesn't have this property, but we set it anyway
            // Users will need to use a custom shader or ShaderGraph that has this property
            material.SetFloat("_PulseSpeed", 1.0f);

            AssetDatabase.CreateAsset(material, assetPath);
            Debug.Log($"[Gateway Setup] Created Material at {assetPath}");
            Debug.Log($"[Gateway Setup] Note: The Standard shader doesn't have a _PulseSpeed property. Consider creating a custom shader or ShaderGraph material.");
            EditorGUIUtility.PingObject(material);
            return true;
        }

        private static bool CreateExampleScene()
        {
            string scenePath = $"{ScenesPath}/{ExampleSceneName}.unity";
            
            // Check if scene already exists
            if (File.Exists(scenePath))
            {
                Debug.Log($"[Gateway Setup] Scene already exists at {scenePath}. Opening existing scene.");
                EditorSceneManager.OpenScene(scenePath);
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                EditorGUIUtility.PingObject(sceneAsset);
                return false;
            }

            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Create Gateway GameObject
            GameObject gatewayObject = new GameObject("Gateway");
            
            // Add AudioSource
            var audioSource = gatewayObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;

            // Add GatewayStateMachine
            var stateMachine = gatewayObject.AddComponent<GatewayStateMachine>();
            
            // Add GatewayVisualController
            var visualController = gatewayObject.AddComponent<GatewayVisualController>();
            
            // Use reflection to set the visual controller reference in the state machine
            var visualControllerField = typeof(GatewayStateMachine).GetField("visualController",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (visualControllerField != null)
            {
                visualControllerField.SetValue(stateMachine, visualController);
            }

            // Load and assign the timeline we created
            string timelineAssetPath = $"{TimelinesPath}/{ExampleTimelineName}.asset";
            var timeline = AssetDatabase.LoadAssetAtPath<GatewaySessionTimeline>(timelineAssetPath);
            if (timeline != null)
            {
                var sessionTimelineField = typeof(GatewayStateMachine).GetField("sessionTimeline",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (sessionTimelineField != null)
                {
                    sessionTimelineField.SetValue(stateMachine, timeline);
                }
            }

            // Load and assign the material we created
            string materialAssetPath = $"{MaterialsPath}/{ExampleMaterialName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialAssetPath);
            if (material != null)
            {
                var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (targetMaterialsField != null)
                {
                    var materialsList = new System.Collections.Generic.List<Material> { material };
                    targetMaterialsField.SetValue(visualController, materialsList);
                }
            }

            // Save the scene
            bool saved = EditorSceneManager.SaveScene(newScene, scenePath);
            if (saved)
            {
                Debug.Log($"[Gateway Setup] Created scene at {scenePath}");
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                EditorGUIUtility.PingObject(sceneAsset);
                return true;
            }
            else
            {
                Debug.LogError($"[Gateway Setup] Failed to save scene at {scenePath}");
                return false;
            }
        }

        private static void LogCompletionMessage(bool stateCreated, bool timelineCreated, bool materialCreated, bool sceneCreated)
        {
            Debug.Log("=== Gateway Setup Wizard Complete ===");
            
            if (stateCreated || timelineCreated || materialCreated || sceneCreated)
            {
                Debug.Log("[Gateway Setup] New assets created:");
                if (stateCreated) Debug.Log($"  - Visual State: {StatesPath}/{ExampleStateName}.asset");
                if (timelineCreated) Debug.Log($"  - Timeline: {TimelinesPath}/{ExampleTimelineName}.asset");
                if (materialCreated) Debug.Log($"  - Material: {MaterialsPath}/{ExampleMaterialName}.mat");
                if (sceneCreated) Debug.Log($"  - Scene: {ScenesPath}/{ExampleSceneName}.unity");
            }
            else
            {
                Debug.Log("[Gateway Setup] All assets already exist. No new assets were created.");
            }

            Debug.Log("\n=== Next Steps ===");
            Debug.Log("1. Create a URP Pipeline Asset:");
            Debug.Log("   - Right-click in Project window > Create > Rendering > URP Asset (with Universal Renderer)");
            Debug.Log("   - Assign it in Edit > Project Settings > Graphics > Scriptable Render Pipeline Settings");
            Debug.Log("\n2. Assign an audio clip:");
            Debug.Log($"   - Open the {ExampleSceneName} scene");
            Debug.Log("   - Select the Gateway GameObject");
            Debug.Log("   - In the AudioSource component, assign your Gateway session audio clip");
            Debug.Log("\n3. (Optional) Replace the material:");
            Debug.Log($"   - Create a custom ShaderGraph material with a _PulseSpeed float property");
            Debug.Log("   - Assign it to the GatewayVisualController's Target Materials list");
            Debug.Log("\n4. (Optional) Add Sentis AI integration:");
            Debug.Log("   - Import an ONNX model and create a ModelAsset");
            Debug.Log("   - Add the BreathingModelController component to the scene");
            Debug.Log("   - Wire the AI output to modulate the visuals");
            Debug.Log("\nFor more information, see Docs/PROJECT_READINESS_CHECKLIST.md");
        }
    }
}
