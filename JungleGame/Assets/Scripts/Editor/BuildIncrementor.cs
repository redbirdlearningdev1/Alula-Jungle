using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildIncrementor : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();

        Debug.Log("current build platform: " + report.summary.platform);

        BuildScriptableObject obj = Resources.Load("Build", typeof(BuildScriptableObject)) as BuildScriptableObject;
        string preNum = obj.buildNumber;
        string postNum = obj.buildNumber;

        switch (report.summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);

                buildScriptableObject.buildNumber = PlayerSettings.macOS.buildNumber;
                postNum = PlayerSettings.macOS.buildNumber;
                break;

            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);

                buildScriptableObject.buildNumber = PlayerSettings.iOS.buildNumber;
                postNum = PlayerSettings.iOS.buildNumber;
                break;

            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode++;

                buildScriptableObject.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                postNum = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
        }

        // determine if number changed
        if (preNum == postNum)
        {
            Debug.LogError("build number not increment - check BuildIncrementor.cs to fix");
        }

        Debug.Log("new build created, updating build number: " + buildScriptableObject.buildNumber);
        
        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset");
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset");
        AssetDatabase.SaveAssets();
    }

    private string IncrementBuildNumber(string buildNumber)
    {
        int.TryParse(buildNumber, out int outputBuildNumber);
        return (outputBuildNumber + 1).ToString();
    }
}
