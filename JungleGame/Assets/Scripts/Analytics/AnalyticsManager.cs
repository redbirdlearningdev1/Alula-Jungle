using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Core.Analytics;
using Unity.Services.Core.Environments;		
using Unity.Services.Analytics;


public class AnalyticsManager : MonoBehaviour
{
    async void Start()
    {
        try
        {
            var options = new InitializationOptions();
            options = options.SetOption("com.unity.services.core.environment-name", true); // set environment id
            options = options.SetOption("com.unity.services.core.analytics-user-id", true); // set custom user id
            

            if (Application.isEditor)
            {
                options.SetEnvironmentName("development"); // set environment name 
            }
            

            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            if (StudentInfoSystem.GetCurrentProfile().active)
            {
                options.SetAnalyticsUserId("player:" + data.name + "_index:" + data.studentIndex.ToString()); // set custom user id
            }
                    
            await UnityServices.InitializeAsync(options);
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();

            // // send a custom event
            // Dictionary<string, object> parameters = new Dictionary<string, object>()
            // {
            //     { "test_parameter", "beep boop beep boop i think this is working! :O" },
            // };            
            // AnalyticsService.Instance.CustomData("test_event", parameters);
            // AnalyticsService.Instance.Flush(); // send event immediately

            GameManager.instance.SendLog("AnalyticsManager", "finished setting up analytics");
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            GameManager.instance.SendError("AnalyticsManager", e.Reason.ToString());
        }
    }
}
