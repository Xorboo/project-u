using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public static class BuildScript
    {
        [MenuItem("Build/WebGL (Development)")]
        public static void BuildWebGlDevelopment()
        {
            var options = WebGlPLayerOptions;
            options.options = BuildOptions.Development;

            Build(options);
        }

        [MenuItem("Build/WebGL (Production)")]
        public static void BuildWebGlProduction()
        {
            var options = WebGlPLayerOptions;
            options.options = BuildOptions.None;

            Build(options);
        }

        [MenuItem("Build/Windows (Development)")]
        public static void BuildWindowsDevelopment()
        {
            var options = WindowsPLayerOptions;
            options.options = BuildOptions.Development;

            BuildWindows(options);
        }

        [MenuItem("Build/Windows (Production)")]
        public static void BuildWindowsProduction()
        {
            var options = WindowsPLayerOptions;
            options.options = BuildOptions.None;

            BuildWindows(options);
        }

        [MenuItem("Build/MacOS (Development)")]
        public static void BuildMacOsDevelopment()
        {
            var options = MacOsPLayerOptions;
            options.options = BuildOptions.Development;

            BuildMacOs(options);
        }

        [MenuItem("Build/MacOS (Production)")]
        public static void BuildMacOsProduction()
        {
            var options = MacOsPLayerOptions;
            options.options = BuildOptions.None;

            BuildMacOs(options);
        }

        static void BuildWindows(BuildPlayerOptions buildPlayerOptions)
        {
            string baseWindowsDir = "Build/Windows";
            if (Directory.Exists(baseWindowsDir))
                Directory.Delete(baseWindowsDir, true);

            Build(buildPlayerOptions);

            string rootDir = Path.GetDirectoryName(buildPlayerOptions.locationPathName);
            string projectName = Path.GetFileNameWithoutExtension(buildPlayerOptions.locationPathName);
            string pdbDir = $"{projectName}_BackUpThisFolder_ButDontShipItWithYourGame";
            string pdbDirectoryPath = Path.Combine(rootDir, pdbDir);
            if (Directory.Exists(pdbDirectoryPath))
            {
                Debug.Log($"Removing pdb dir: {pdbDirectoryPath}");
                Directory.Delete(pdbDirectoryPath, true);
            }
            else
                Debug.LogWarning($"Couldn't find pdb dir to remove: {pdbDirectoryPath}");
        }

        static void BuildMacOs(BuildPlayerOptions buildPlayerOptions)
        {
            var defaultStandaloneScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone);
            try
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                Build(buildPlayerOptions);
            }
            finally
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, defaultStandaloneScriptingBackend);
            }
        }

        static void Build(BuildPlayerOptions buildPlayerOptions)
        {
            if (AddressableAssetSettingsDefaultObject.Settings != null)
            {
                AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
                AddressableAssetSettings.BuildPlayerContent(out var result);
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.LogError($"Addressable content build failure: {result.Error}");
                    return;
                }
            }
            else
                Debug.Log("Addressable settings object is null, can't build it");

            buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded: {summary.totalSize} bytes, path: {Path.GetFullPath(buildPlayerOptions.locationPathName)}");
                    break;

                case BuildResult.Failed:
                    Debug.LogError($"Build failed");
                    break;
            }
        }

        static BuildPlayerOptions WebGlPLayerOptions => new()
        {
            target = BuildTarget.WebGL,
            locationPathName = "Build/WebGL/",
            scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray()
        };

        static BuildPlayerOptions WindowsPLayerOptions => new()
        {
            target = BuildTarget.StandaloneWindows64,
            locationPathName = "Build/Windows/RollAndCrawl/RollAndCrawl.exe",
            scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray()
        };

        static BuildPlayerOptions MacOsPLayerOptions => new()
        {
            target = BuildTarget.StandaloneOSX,
            locationPathName = "Build/MacOS/RollAndCrawl",
            scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray()
        };
    }
}