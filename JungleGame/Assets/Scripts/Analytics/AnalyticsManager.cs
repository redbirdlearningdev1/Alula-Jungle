using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Core.Analytics;
using Unity.Services.Core.Environments;		
using Unity.Services.Analytics;
using Unity.Services.Authentication;


public class AnalyticsManager : MonoBehaviour
{
    public static bool useTestingEnvironment = true;
    
    private static string currentProfile = "";
    
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

    public static void SetAnalyticsOption(bool opt)
    {
        // opt into using analytics
        if (opt)
        {
            SetUpAnalytics();
        }
        // opt out
        else
        {
            AnalyticsService.Instance.OptOut();
            Debug.Log("opted out of analytics");
        }
    }

    public static void SendCustomEvent(string eventName, Dictionary<string, object> parameters)
    {
        AnalyticsService.Instance.CustomData(eventName, parameters);
        AnalyticsService.Instance.Flush(); // send event immediately
    }

    public static async void SwitchProfile(string newProfile)
    {
        // if new profile is current profile - return || if unity services not initialized - return
        if (newProfile == currentProfile || UnityServices.State != ServicesInitializationState.Initialized)
        {
            return;
        }

        // sign out
        AuthenticationService.Instance.SignOut();

        // switch profile
        try
        {
            AuthenticationService.Instance.SwitchProfile(newProfile);
            currentProfile = newProfile;
            Debug.Log("switched to new profile: " + newProfile);
        }
        catch (AuthenticationException e)
        {
            Debug.LogException(e);
        }

        // sign back in
        try
        {
            // SignInOptions signInOptions = new SignInOptions();
            // signInOptions.CreateAccount = true;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("sign in anonymously succeeded!");

            //// ANALYTICS : send player_login event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "authentication_player_id", AuthenticationService.Instance.PlayerId },
                { "profile_name", data.name },
                { "student_index", ((int)data.studentIndex + 1) },
                { "unique_id", data.uniqueID }
            };            
            SendCustomEvent("player_login", parameters);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public static async void SetUpAnalytics()
    {
        InitializationOptions options = new InitializationOptions();
        options = options.SetOption("com.unity.services.core.environment-name", true); // set environment id
        //options = options.SetOption("com.unity.services.core.analytics-user-id", true); // set custom user id

        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        if (data != null || data.active)
        {
            // create ID if null
            if (data.uniqueID == null)
            {
                StudentInfoSystem.GetCurrentProfile().uniqueID = LoadSaveSystem.CreateNewUniqueID();
                StudentInfoSystem.SaveStudentPlayerData();
            }

            currentProfile = data.name + "_" + data.uniqueID;
        }
        else
        {
            currentProfile = "default_profile";
        }

        options.SetProfile(currentProfile); // set profile
        Debug.Log("initialized profile as: " + currentProfile);

        // set environment name 
        if (Application.isEditor)
        {
            options.SetEnvironmentName("development"); 
        }
        else if (useTestingEnvironment)
        {
            options.SetEnvironmentName("testing"); 
        }

        // update unity services
        try
        {
            await UnityServices.InitializeAsync(options);
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();

            // anonymous sign-in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("initial sign in anonymously succeeded!");

            //// ANALYTICS : send player_login event
            // set parameters to default value in case profile is null
            string profile_name = "no_profile_selected";
            int student_index = -1;
            string unique_id = "no_unique_id";

            // switch parameters using profile data
            if (data != null || data.active)
            {
                profile_name = data.name;
                student_index = ((int)data.studentIndex + 1);
                unique_id = data.uniqueID;
            }

            // send player_login event
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "authentication_player_id", AuthenticationService.Instance.PlayerId },
                { "profile_name", profile_name },
                { "student_index", student_index },
                { "unique_id", unique_id }
            };            
            SendCustomEvent("player_login", parameters);
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            Debug.LogException(e);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
