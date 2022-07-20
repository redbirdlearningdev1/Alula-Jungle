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

        switch (report.summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSX:
                Debug.Log("pre num: " + PlayerSettings.macOS.buildNumber);
                PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
                Debug.Log("post num: " + PlayerSettings.macOS.buildNumber);

                buildScriptableObject.buildNumber = PlayerSettings.macOS.buildNumber;
                break;

            case BuildTarget.iOS:
                Debug.Log("pre num: " + PlayerSettings.iOS.buildNumber);
                PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
                Debug.Log("post num: " + PlayerSettings.iOS.buildNumber);

                buildScriptableObject.buildNumber = PlayerSettings.iOS.buildNumber;
                break;

            case BuildTarget.Android:
                Debug.Log("pre num: " + PlayerSettings.Android.bundleVersionCode.ToString());
                PlayerSettings.Android.bundleVersionCode++;
                Debug.Log("post num: " + PlayerSettings.Android.bundleVersionCode.ToString());
                
                buildScriptableObject.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
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
