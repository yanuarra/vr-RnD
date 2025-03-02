using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VRBuilder.Pro.Editor.Demo
{
    /// <summary>
    /// Menu item for loading demo scenes after checking the process file is in the StreamingAssets folder.
    /// </summary>
    public static class DemoSceneLoader
    {
        private static bool IsUpmPackage => File.Exists("Packages/co.mindport.vrbuilder.pro/Demo/Editor/DemoSceneLoader.cs");

        [MenuItem("Tools/VR Builder/Demo Scenes/Animations", false, 64)]
        public static void LoadAnimationsDemoScene()
        {
            LoadDemoScene("VR Builder Demo - Animations", "Demo - Animations");
        }

        [MenuItem("Tools/VR Builder/Demo Scenes/Highlights and Guidance", false, 64)]
        public static void LoadHighlightsDemoScene()
        {
            LoadDemoScene("VR Builder Demo - Highlights and Guidance", "Demo - Highlights and Guidance");
        }


        //[MenuItem("Tools/VR Builder/Demo Scenes/Randomization", false, 64)]
        public static void LoadRandomizationDemoScene()
        {
            LoadDemoScene("VR Builder Demo - Randomization", "Demo - Randomization");
        }


        //[MenuItem("Tools/VR Builder/Demo Scenes/States and Data", false, 64)]
        public static void LoadStatesAndDataDemoScene()
        {
            LoadDemoScene("VR Builder Demo - States and Data", "Demo - States and Data");
        }

        [MenuItem("Tools/VR Builder/Demo Scenes/Track and Measure", false, 64)]
        public static void LoadTrackAndMeasureDemoScene()
        {
            LoadDemoScene("VR Builder Demo - Track and Measure", "Demo - Track and Measure");
        }

        public static void LoadDemoScene(string sceneName, string processName)
        {
#if !VR_BUILDER_XR_INTERACTION
            if (EditorUtility.DisplayDialog("XR Interaction Component Required", "This demo scene requires VR Builder's built-in XR Interaction Component to be enabled. It looks like it is currently disabled. You can enable it in Project Settings > VR Builder > Settings.", "Ok")) 
            {
                return;
            }
#endif
            string demoScenePath = GetDemoScenePath(sceneName);
            string demoProcessPath = GetDemoProcessPath(processName);
            string demoProcessTargetPath = GetDemoProcessTargetPath(processName);
            string demoProcessTargetDirectory = GetDemoProcessTargetDirectory(processName);

            if (File.Exists(demoProcessTargetPath) == false)
            {
                Directory.CreateDirectory(demoProcessTargetDirectory);
                FileUtil.CopyFileOrDirectory(demoProcessPath, demoProcessTargetPath);
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(demoScenePath);

#if VR_BUILDER && VR_BUILDER_XR_INTERACTION
            foreach (GameObject configuratorGameObject in GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).
                Where(go => go.GetComponent<VRBuilder.Core.Setup.ILayerConfigurator>() != null))
            {
                VRBuilder.Core.Setup.ILayerConfigurator configurator = configuratorGameObject.GetComponent<VRBuilder.Core.Setup.ILayerConfigurator>();
                if (configurator.LayerSet == VRBuilder.Core.Setup.LayerSet.Teleportation)
                {
                    configurator.ConfigureLayers("Teleport", "Teleport");
                    EditorUtility.SetDirty(configuratorGameObject);
                }
            }

            EditorSceneManager.SaveOpenScenes();
#endif

            AssetDatabase.Refresh();
        }

        private static string GetDemoProcessTargetDirectory(string processName)
        {
            return $"Assets/StreamingAssets/Processes/{processName}";
        }

        private static string GetDemoProcessTargetPath(string processName)
        {
            return $"Assets/StreamingAssets/Processes/{processName}/{processName}.json";
        }

        private static string GetDemoProcessPath(string processName)
        {
            if (IsUpmPackage)
            {
                return $"Packages/co.mindport.vrbuilder.pro/Demo/StreamingAssets/Processes/{processName}/{processName}.json";
            }
            else
            {
                return $"Assets/MindPort/VR Builder/Pro/Demo/StreamingAssets/Processes/{processName}/{processName}.json";
            }
        }

        private static string GetDemoScenePath(string sceneName)
        {
            if (IsUpmPackage)
            {
                return $"Packages/co.mindport.vrbuilder.pro/Demo/Scenes/{sceneName}.unity";
            }
            else
            {
                return $"Assets/MindPort/VR Builder/Pro/Demo/Scenes/{sceneName}.unity";
            }
        }
    }
}