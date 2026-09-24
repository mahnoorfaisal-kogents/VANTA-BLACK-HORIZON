#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Vanta.EditorTools
{
    public static class VantaWindowsBuild
    {
        const string OutputDirectory = "Builds/Windows";
        const string OutputPath = OutputDirectory + "/VANTA_BLACK_HORIZON.exe";

        [MenuItem("VANTA/Build Windows x64")]
        public static void BuildWindows()
        {
            Directory.CreateDirectory(OutputDirectory);
            var scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0)
            {
                Debug.LogError("VANTA build aborted: no enabled scenes are configured in Build Settings.");
                return;
            }

            var enabledScenes = System.Array.FindAll(scenes, scene => scene.enabled);
            if (enabledScenes.Length == 0)
            {
                Debug.LogError("VANTA build aborted: no enabled scenes are configured in Build Settings.");
                return;
            }

            var options = new BuildPlayerOptions
            {
                scenes = System.Array.ConvertAll(enabledScenes, scene => scene.path),
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError($"VANTA Windows build failed: {report.summary.result}");
                return;
            }

            Debug.Log($"VANTA Windows build succeeded: {Path.GetFullPath(OutputPath)}");
        }
    }
}
