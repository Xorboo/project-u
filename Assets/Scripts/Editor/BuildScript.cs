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
            var options = DefaultPLayerOptions;
            options.options = BuildOptions.Development;

            BuildWebGl(options);
        }

        [MenuItem("Build/WebGL (Production)")]
        public static void BuildWebGlProduction()
        {
            var options = DefaultPLayerOptions;
            options.options = BuildOptions.None;

            BuildWebGl(options);
        }

        static void BuildWebGl(BuildPlayerOptions buildPlayerOptions)
        {
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent(out var result);
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.LogError($"Addressable content build failure: {result.Error}");
                return;
            }

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded: {summary.totalSize} bytes");
                    break;

                case BuildResult.Failed:
                    Debug.LogError($"Build failed");
                    break;
            }
        }

        static BuildPlayerOptions DefaultPLayerOptions => new BuildPlayerOptions
        {
            target = BuildTarget.WebGL,
            locationPathName = "Build/WebGL",
            scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray()
        };
    }
}