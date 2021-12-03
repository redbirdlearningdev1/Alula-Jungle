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
    public const string challenge_word_audio_folder = "Assets/Resources/ChallengeWordAudio/";
    public const string challenge_word_pair_folder = "Assets/Resources/WordPairObjects/";

    public const string item_postfix = "_cw_obj";
    public const string sprite_postfix = "_cwimg";
    public const string audio_postfix = "_audio";
    public const string pair_postfix = "_pair";
    
    public const int elkonin_word_separator = 21; // value at which > is consonant coins & <= are action word coins

    public static List<ChallengeWord> globalChallengeWordList;
    public static List<WordPair> globalWordPairs;

    public static void InitCreateGlobalList(bool remove_incomplete_words = false)
    {
        var words = Resources.LoadAll<ChallengeWord>("ChallengeWordObjects");

        globalChallengeWordList = new List<ChallengeWord>();
        foreach (var word in words)
        {
            if (remove_incomplete_words)
            {
                if (word.audio != null && word.sprite != null)
                    globalChallengeWordList.Add(word);
            }
            else
            {
                globalChallengeWordList.Add(word);
            }
        }
    }

    public static void InitCreateGlobalPairList()
    {
        var words = Resources.LoadAll<WordPair>("WordPairObjects");

        globalWordPairs = new List<WordPair>();
        foreach (var word in words)
        {
            globalWordPairs.Add(word);
        }
    }

#if UNITY_EDITOR
    public static void UpdateCreateWord(ChallengeWordEntry data)
    {   
        string exact_filename = data.word + item_postfix;
        string[] results = AssetDatabase.FindAssets(exact_filename);
        string result = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == exact_filename + ".asset")
            {
                result = path;
                Debug.Log("result: " + result);
            }
        }

        ChallengeWord yourObject = null;

        // create new challenge word
        if(result == "")
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "!!! creating new challenge word -> " + data.word + item_postfix);
            yourObject = ScriptableObject.CreateInstance<ChallengeWord>();
            AssetDatabase.CreateAsset(yourObject, challenge_word_folder + data.word + item_postfix + ".asset");
        }
        // get challenge word object
        else
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "%%% found challenge word -> " + data.word + item_postfix);
            yourObject = (ChallengeWord)AssetDatabase.LoadAssetAtPath(result, typeof(ChallengeWord));
        }

        GameManager.instance.SendLog("ChallengeWordDatabase", "yourObject -> " + yourObject.word);

        // Do your changes
        yourObject.word = data.word;
        yourObject.elkoninList = data.elkoninList;
        yourObject.elkoninCount = data.elkoninCount;
        yourObject.set = data.set;
        Debug.Log("data set: " + data.set);

        // find sprite image
        yourObject.sprite = FindWordSprite(data.word);

        // find word audio
        yourObject.audio = FindWordAudio(data.word);

        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void UpdateCreatePair(ChallengePairEntry entry)
    {
        Debug.Log(entry.word1.word + " & " + entry.word2.word + " - pair type:" + entry.pairType);

        string exact_filename = entry.word1.word + "_" + entry.word2.word + pair_postfix;
        string[] results = AssetDatabase.FindAssets(exact_filename);
        string result = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == exact_filename + ".asset")
            {
                result = path;
                Debug.Log("result: " + result);
            }
        }

        WordPair yourObject = null;

        // create new word pair object
        if(result == "")
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "!!! creating new challenge word pair -> " + exact_filename);
            yourObject = ScriptableObject.CreateInstance<WordPair>();
            AssetDatabase.CreateAsset(yourObject, challenge_word_pair_folder + exact_filename + ".asset");
        }
        // get word pair object
        else
        {
            GameManager.instance.SendLog("ChallengeWordDatabase", "%%% found challenge word pair -> " + exact_filename);
            yourObject = (WordPair)AssetDatabase.LoadAssetAtPath(result, typeof(WordPair));
        }

        //GameManager.instance.SendLog("ChallengeWordDatabase", "yourObject -> " + yourObject.word);

        // Do your changes
        yourObject.soundCoin = entry.soundCoin;
        yourObject.pairType = entry.pairType;
        yourObject.word1 = entry.word1;
        yourObject.word2 = entry.word2;
        yourObject.index = entry.index;

        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static AudioClip FindWordAudio(string word)
    {
        string exact_filename = word + audio_postfix;
        string[] results = AssetDatabase.FindAssets(exact_filename);
        string correct_path = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == exact_filename + ".wav")
            {
                correct_path = path;
                //Debug.Log ("audio correct_path: " + correct_path);
            }
        }

        // found correct audio file
        if (correct_path != "")
        {
            return (AudioClip)AssetDatabase.LoadAssetAtPath(correct_path, typeof(AudioClip));
        }
        else
        {
            GameManager.instance.SendError("ChallengeWordDatabase", "could not find audio -> " + word + audio_postfix);
        }

        return null;
    }

    private static Sprite FindWordSprite(string word)
    {
        string exact_filename = word + sprite_postfix;
        string[] results = AssetDatabase.FindAssets(word + sprite_postfix); 
        string correct_path = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == exact_filename + ".png")
            {
                correct_path = path;
                //Debug.Log ("sprint correct_path: " + correct_path);
            }
        }

        // found correct sprite
        if (correct_path != "")
        {
            return (Sprite)AssetDatabase.LoadAssetAtPath(correct_path, typeof(Sprite));
        }
        // place default image
        else 
        {
            GameManager.instance.SendError("ChallengeWordDatabase", "could not find sprite -> " + word + sprite_postfix);
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

    public static ElkoninValue ActionWordEnumToElkoninValue(ActionWordEnum value)
    {
        switch (value)
        {
            default:
            case ActionWordEnum._blank:     return ElkoninValue.empty_gold;
            case ActionWordEnum.mudslide:   return ElkoninValue.mudslide;
            case ActionWordEnum.listen:     return ElkoninValue.listen;
            case ActionWordEnum.poop:       return ElkoninValue.poop;
            case ActionWordEnum.orcs:       return ElkoninValue.orcs;
            case ActionWordEnum.think:      return ElkoninValue.think;
            case ActionWordEnum.hello:      return ElkoninValue.hello;
            case ActionWordEnum.spider:     return ElkoninValue.spider;
            case ActionWordEnum.explorer:   return ElkoninValue.explorer;
            case ActionWordEnum.scared:     return ElkoninValue.scared;
            case ActionWordEnum.thatguy:    return ElkoninValue.thatguy;
            case ActionWordEnum.choice:     return ElkoninValue.choice;
            case ActionWordEnum.strongwind: return ElkoninValue.strongwind;
            case ActionWordEnum.pirate:     return ElkoninValue.pirate;
            case ActionWordEnum.gorilla:    return ElkoninValue.gorilla;
            case ActionWordEnum.sounds:     return ElkoninValue.sounds;
            case ActionWordEnum.give:       return ElkoninValue.give;
            case ActionWordEnum.backpack:   return ElkoninValue.backpack;
            case ActionWordEnum.frustrating:return ElkoninValue.frustrating;
            case ActionWordEnum.bumphead:   return ElkoninValue.bumphead;
            case ActionWordEnum.baby:       return ElkoninValue.baby;
        }
    }
}
