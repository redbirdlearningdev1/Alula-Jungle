using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Core.Analytics;
using Unity.Services.Core.Environments;		
using Unity.Services.Analytics;


public class AnalyticsManager : MonoBehaviour
{
    public static bool useTestingEnvironment = true;
    private static InitializationOptions options;
    
    void Start()
    {
        if (PlayerPrefs.GetInt("USE_ANALYTICS") == 1)
        {
            SetAnalyticsOption(true);
        }
        else
        {
            SetAnalyticsOption(false);
        }
    }

    private static async void TurnOnAnalytics()
    {
        try
        {
            options = new InitializationOptions();
            options = options.SetOption("com.unity.services.core.environment-name", true); // set environment id
            options = options.SetOption("com.unity.services.core.analytics-user-id", true); // set custom user id
            
            // set environment name 
            if (Application.isEditor)
            {
                options.SetEnvironmentName("development"); 
            }
            else if (useTestingEnvironment)
            {
                options.SetEnvironmentName("testing"); 
            }
            
            UpdateUserID();

            await UnityServices.InitializeAsync(options);
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();

            GameManager.instance.SendLog("AnalyticsManager", "finished setting up analytics");
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            GameManager.instance.SendError("AnalyticsManager", e.Reason.ToString());
        }
    }

    public static void SetAnalyticsOption(bool opt)
    {
        // opt into using analytics
        if (opt)
        {
            TurnOnAnalytics();
        }
        // opt out
        else
        {
            AnalyticsService.Instance.OptOut();
        }
    }

    public static void SendCustomEvent(string eventName, Dictionary<string, object> parameters)
    {
        AnalyticsService.Instance.CustomData(eventName, parameters);
        AnalyticsService.Instance.Flush(); // send event immediately
    }

    public static void UpdateUserID()
    {
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        if (data.active)
        {
            // create ID if null
            if (data.uniqueID == null)
            {
                StudentInfoSystem.GetCurrentProfile().uniqueID = LoadSaveSystem.CreateNewUniqueID();
                StudentInfoSystem.SaveStudentPlayerData();
            }

            options.SetAnalyticsUserId(data.name + ":" + data.studentIndex.ToString() + ":" + data.uniqueID); // set custom user id
        }
        else
        {
            options.SetAnalyticsUserId("new_user:" + LoadSaveSystem.CreateNewUniqueID());
        }
    }
}
