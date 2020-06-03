using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class LoadSaveSystem
{
    public static void SaveStudentData(StudentPlayerData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string path = GetStudentDataPath(data.studentIndex);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(jsonData);
                writer.Close();
            }
        }
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
            Debug.Log("Path not found: " + path.ToString() + "\nReturning null");
        }

        return null;
    }

    private static string GetStudentDataPath(StudentIndex index)
    {
        string path = "";
        
        if (Application.isEditor)
            path = "Assets/Resources/" + index.ToString() + "_data.json"; // example student_2_data.json
        else
            path = "JungleGame_Data/Resources/" + index.ToString() + "_data.json";

        return path;
    }
}
