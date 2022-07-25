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

            Build(options);
        }

        [MenuItem("Build/Windows (Production)")]
        public static void BuildWindowsProduction()
        {
            var options = WindowsPLayerOptions;
            options.options = BuildOptions.None;

            Build(options);
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
    }
}