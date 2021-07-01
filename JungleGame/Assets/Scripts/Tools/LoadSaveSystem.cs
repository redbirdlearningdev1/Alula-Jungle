using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LoadSaveSystem
{
    // default student player values
    public static string default_version = "1.2";
    public static string default_name = "empty";
    public static int    default_stars = 0;
    public static int    default_map_limit = 0;

    public static float default_masterVol = 1f;
    public static float default_musicVol = 1f;
    public static float default_fxVol = 1f;
    public static float default_talkVol = 1f;
    public static int default_micDevice = 0;

    public static void SaveStudentData(StudentPlayerData data, bool makeInactive = false)
    {
        // saving data to profile makes it active
        if (!data.active)
            data.active = true;
        if (makeInactive)
            data.active = false;

        string jsonData = JsonUtility.ToJson(data);
        string path = GetStudentDataPath(data.studentIndex);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                Debug.Log("json data: " + jsonData);
                writer.Write(jsonData);
                writer.Close();
            }
        }
        // refresh database
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public static StudentPlayerData LoadStudentData(StudentIndex index)
    {
        string path = GetStudentDataPath(index);
        string file = "{}";

        if (System.IO.File.Exists(path))
        {
            // Read the json text from directly from the *.txt file
            using (StreamReader reader = new StreamReader(path))
            {
                file = reader.ReadToEnd();
                reader.Close();
            }
            
            StudentPlayerData studentData = JsonUtility.FromJson<StudentPlayerData>(file);
            if (studentData != null)
                return studentData;
            else
                Debug.Log("StudentPlayerData is null, returning null.");
        }
        else
        {
            // Debug.Log("Path not found: " + path.ToString() + "\nReturning null");
            // create new profile file
            ResetStudentData(index);
            return LoadStudentData(index);
        }

        return null;
    }

    public static void ResetStudentData(StudentIndex index)
    {
        StudentPlayerData new_data = new StudentPlayerData();
        new_data.studentIndex = index;

        // set all variables to be default values
        new_data.version = default_version;
        new_data.name = default_name;
        new_data.totalStars = default_stars;

        new_data.masterVol = default_masterVol;
        new_data.musicVol = default_musicVol;
        new_data.fxVol = default_fxVol;
        new_data.talkVol = default_talkVol;
        new_data.micDevice = default_micDevice;

        // save data as incative profile
        SaveStudentData(new_data, true);
        GameManager.instance.SendLog("LoadSaveSystem", "reseting profile " + index);
    }

    private static string GetStudentDataPath(StudentIndex index)
    {
        string path = "";
        
        if (Application.isEditor)
            path = "Assets/Resources/" + index.ToString() + "_data.json"; // example student_2_data.json
        else
            path = Application.persistentDataPath + "/" + index.ToString() + "_data.json";

        return path;
    }
}
