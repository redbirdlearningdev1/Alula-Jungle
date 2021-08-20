using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ChallengeWordDatabase
{
    public const string challenge_word_folder = "Assets/Resources/ChallengeWordObjects/";
    public const string challenge_word_sprites_folder = "Assets/Resources/PolaroidImages/WebImages/";
    public const string item_postfix = "_cw_obj";
    public const string sprite_postfix = "_cwimg";
    public const int elkonin_word_separator = 21; // value at which > is consonant coins & <= are action word coins

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

#if UNITY_EDITOR
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
        yourObject.elkoninCount = data.elkoninCount;
        yourObject.imageText = data.imageText;
        yourObject.set = data.set;

        // find sprite image
        yourObject.sprite = FindWordSprite(data.word);

        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Sprite FindWordSprite(string word)
    {
        string[] result = AssetDatabase.FindAssets(word + sprite_postfix);

        foreach (var res in result)
        {
            Debug.Log("res: " + res);
        }

        // found correct sprite
        if (result.Length == 1)
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "found challenge word sprite -> " + word + sprite_postfix);
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            return (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
        }
        // place default image
        else 
        {
            
        }

        return null;
    }
#endif

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

    public static ConsonantEnum ElkoninValueToConsonantEnum(ElkoninValue value)
    {
        switch (value)
        {
            case ElkoninValue.empty_silver:
                return ConsonantEnum._blank;
            case ElkoninValue.b: 
                return ConsonantEnum.b;
            case ElkoninValue.c:
                return ConsonantEnum.c;
            case ElkoninValue.ch:
                return ConsonantEnum.ch;
            case ElkoninValue.d:
                return ConsonantEnum.d;
            case ElkoninValue.f:
                return ConsonantEnum.f;
            case ElkoninValue.g:
                return ConsonantEnum.g;
            case ElkoninValue.h:
                return ConsonantEnum.h;
            case ElkoninValue.j:
                return ConsonantEnum.j;
            case ElkoninValue.k:
                return ConsonantEnum.k;
            case ElkoninValue.l:
                return ConsonantEnum.l;
            case ElkoninValue.m:
                return ConsonantEnum.m;
            case ElkoninValue.n:
                return ConsonantEnum.n;
            case ElkoninValue.p:
                return ConsonantEnum.p;
            case ElkoninValue.qu:
                return ConsonantEnum.qu;
            case ElkoninValue.r:
                return ConsonantEnum.r;
            case ElkoninValue.s: 
                return ConsonantEnum.s;
            case ElkoninValue.sh:
                return ConsonantEnum.sh;
            case ElkoninValue.t:
                return ConsonantEnum.t;
            case ElkoninValue.th:
                return ConsonantEnum.th;
            case ElkoninValue.v:
                return ConsonantEnum.v;
            case ElkoninValue.w:
                return ConsonantEnum.w;
            case ElkoninValue.x:
                return ConsonantEnum.x;
            case ElkoninValue.y:
                return ConsonantEnum.y;
            case ElkoninValue.z:
                return ConsonantEnum.z;
            default:
                GameManager.instance.SendError("ChallengeWordDatabase", "invalid ElkoninValue to ConsonantEnum");
                return ConsonantEnum._blank;

        }
    }

    public static ActionWordEnum ElkoninValueToActionWord(ElkoninValue value)
    {
        switch (value)
        {
            case ElkoninValue.empty_gold:
            case ElkoninValue.empty_silver:
                return ActionWordEnum._blank;
            case ElkoninValue.mudslide:
                return ActionWordEnum.mudslide;
            case ElkoninValue.listen:
                return ActionWordEnum.listen;
            case ElkoninValue.poop:
                return ActionWordEnum.poop;
            case ElkoninValue.orcs:
                return ActionWordEnum.orcs;
            case ElkoninValue.think:
                return ActionWordEnum.think;
            case ElkoninValue.hello:
                return ActionWordEnum.hello;
            case ElkoninValue.spider:
                return ActionWordEnum.spider;
            case ElkoninValue.explorer:
                return ActionWordEnum.explorer;
            case ElkoninValue.scared:
                return ActionWordEnum.scared;
            case ElkoninValue.thatguy:
                return ActionWordEnum.thatguy;
            case ElkoninValue.choice:
                return ActionWordEnum.choice;
            case ElkoninValue.strongwind:
                return ActionWordEnum.strongwind;
            case ElkoninValue.pirate:
                return ActionWordEnum.pirate;
            case ElkoninValue.gorilla:
                return ActionWordEnum.gorilla;
            case ElkoninValue.sounds:
                return ActionWordEnum.sounds;
            case ElkoninValue.give:
                return ActionWordEnum.give;
            case ElkoninValue.backpack:
                return ActionWordEnum.backpack;
            case ElkoninValue.frustrating:
                return ActionWordEnum.frustrating;
            case ElkoninValue.bumphead:
                return ActionWordEnum.bumphead;
            case ElkoninValue.baby:
                return ActionWordEnum.baby;
            default:
                GameManager.instance.SendError("ChallengeWordDatabase", "invalid ElkoninValue to ActionWordEnum: " + value);
                return ActionWordEnum._blank;
        }
    }
}
