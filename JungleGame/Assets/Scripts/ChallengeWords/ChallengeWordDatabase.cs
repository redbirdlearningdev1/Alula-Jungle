using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ChallengeWordDatabase
{
    public const string challenge_word_folder = "Assets/Resources/ChallengeWordObjects/";
    public const string item_postfix = "_cw_obj";

    public static List<ChallengeWord> globalChallengeWordList;

    public static void InitCreateGlobalList()
    {
        var words = Resources.LoadAll<ChallengeWord>("ChallengeWordObjects");

        globalChallengeWordList = new List<ChallengeWord>();
        foreach (var word in words)
        {
            globalChallengeWordList.Add(word);
        }
    }

    public static void UpdateCreateWord(ChallengeWordEntry data)
    {   
        string[] result = AssetDatabase.FindAssets(data.word + item_postfix);
        ChallengeWord yourObject = null;

        if (result.Length > 1)
        {
            GameManager.instance.SendError("ChallengeWordDatabase", "multiple instances of challenge word -> " + data.word + item_postfix);
            return;
        }

        // create new challenge word
        if(result.Length == 0)
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "creating new challenge word -> " + data.word + item_postfix);
            yourObject = ScriptableObject.CreateInstance<ChallengeWord>();
            AssetDatabase.CreateAsset(yourObject, challenge_word_folder + data.word + item_postfix + ".asset");
        }
        // get challenge word object
        else
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "found challenge word -> " + data.word + item_postfix);
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            yourObject = (ChallengeWord)AssetDatabase.LoadAssetAtPath(path, typeof(ChallengeWord));
        }

        GameManager.instance.SendLog("ChallengeWordDatabase", "yourObject -> " + yourObject);

        // Do your changes
        yourObject.word = data.word;
        yourObject.elkoninList = data.elkoninList;
        yourObject.imageText = data.imageText;
        yourObject.set = data.set;


        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static ChallengeWord FindWord(string wordName)
    {
        foreach (var challengeWord in globalChallengeWordList)
        {
            if (challengeWord.word == wordName)
                return challengeWord;
        }

        GameManager.instance.SendError("ChallengeWordDatabase", "invalid challenge word search -> " + wordName);
        return null;
    }
}
