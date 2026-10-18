using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void PerformAndroidBuild()
    {
        Debug.Log("Configuring Android SDK/NDK/JDK paths...");
        string editorPath = "C:/Program Files/Unity/Hub/Editor/6000.3.3f1/Editor/Data/PlaybackEngines/AndroidPlayer";
        string sdkPath = editorPath + "/SDK";
        string ndkPath = editorPath + "/NDK";
        string jdkPath = editorPath + "/OpenJDK";

        EditorPrefs.SetBool("SdkUseEmbedded", true);
        EditorPrefs.SetBool("NdkUseEmbedded", true);
        EditorPrefs.SetBool("JdkUseEmbedded", true);

        EditorPrefs.SetString("AndroidSdkRoot", sdkPath);
        EditorPrefs.SetString("AndroidNdkRoot", ndkPath);
        EditorPrefs.SetString("Jdk17Path", jdkPath);

        System.Environment.SetEnvironmentVariable("ANDROID_HOME", sdkPath);
        System.Environment.SetEnvironmentVariable("ANDROID_SDK_ROOT", sdkPath);
        System.Environment.SetEnvironmentVariable("JAVA_HOME", jdkPath);

        Debug.Log("Switching Active Build Target to Android...");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        string[] scenes = new string[] { "Assets/Application/Scenes/AR_Interaction.unity" };
        string buildPath = "Builds/Android/TaskProject.apk";

        string buildDir = System.IO.Path.GetDirectoryName(buildPath);
        if (!System.IO.Directory.Exists(buildDir))
        {
            System.IO.Directory.CreateDirectory(buildDir);
        }

        EditorUserBuildSettings.buildAppBundle = false; // Generate standalone APK

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        Debug.Log("Starting Android Build to: " + buildPath);
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Android Build Succeeded! Size: {summary.totalSize} bytes at {buildPath}");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"Android Build Failed! Result: {summary.result}, Errors: {summary.totalErrors}");
            EditorApplication.Exit(1);
        }
    }
}
