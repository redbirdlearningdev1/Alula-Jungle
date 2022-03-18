using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct TalkieDatabaseEntry
{
    public Sprite sprite;
    public TalkieCharacter character;
    public int emotionNum;
    public TalkieMouth mouth;
    public TalkieEyes eyes;
}

public class TalkieDatabase : MonoBehaviour
{
    public static TalkieDatabase instance;
    private const string talkie_object_folder = "TalkieObjects/";
    private const string talkie_object_creation_folder = "Assets/Resources/TalkieObjects/";

    /* s
    ################################################
    #   TALKIE SPRITES
    ################################################
    */

    [Header("Character Sprites")]
    public List<TalkieDatabaseEntry> redSprites;
    public List<TalkieDatabaseEntry> darwinSprites;
    public List<TalkieDatabaseEntry> wallySprites;
    public List<TalkieDatabaseEntry> juliusSprites;
    public List<TalkieDatabaseEntry> marcusSprites;
    public List<TalkieDatabaseEntry> brutusSprites;
    public List<TalkieDatabaseEntry> lesterSprites;
    public List<TalkieDatabaseEntry> cloggSprites;
    public List<TalkieDatabaseEntry> bubblesSprites;
    public List<TalkieDatabaseEntry> ollieSprites;
    public List<TalkieDatabaseEntry> spindleSprites;
    public List<TalkieDatabaseEntry> sylvieSprites;
    public List<TalkieDatabaseEntry> celesteSprites;
    public List<TalkieDatabaseEntry> taxiSprites;


    [Header("Talkie Reaction Duplicates")]
    public List<AudioClip> marcusLaughList;
    public List<AudioClip> brutusLaughList;
    public List<AudioClip> marcusArghList;
    public List<AudioClip> marcusGrrList;
    public List<AudioClip> brutusHehList;

    public List<AudioClip> redWallyGaspList;
    public List<AudioClip> redWallyWhatList;
    public List<AudioClip> redWallyHuhList;
    public List<AudioClip> redWallyOhList;
    public List<AudioClip> redWallyDarwinList;

    public List<AudioClip> juliusHahaList;
    public List<AudioClip> juliusAhHahList;
    public List<AudioClip> juliusHrmList;
    public List<AudioClip> juliusUghList;
    public List<AudioClip> juliusGrrList;

    public List<AudioClip> redGaspList;
    public List<AudioClip> redWoohooList;
    public List<AudioClip> redHurrahList;
    public List<AudioClip> redUhHuhList;

    private List<TalkieObject> globalTalkieList; // list of all talkies in this database

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // attempt to find talkie in asset database
    public TalkieObject GetTalkieObject(string talkiename)
    {
        var result = Resources.Load<TalkieObject>(talkie_object_folder + talkiename);

        // return error if none are found
        if (result == null)
        {
            // do nothing - return error and null at the bottom of this function
        }
        // try to find correct talkie - return error if wrong name found
        else 
        {
            if (result.name == talkiename)
                return result;
        }

        GameManager.instance.SendError(this, "could not find talkie: \"" + talkiename + "\"");
        return null;
    }

    public List<TalkieObject> GetGlobalTalkieList()
    {
        TalkieObject[] talkieObjects = Resources.LoadAll<TalkieObject>(talkie_object_folder);

        globalTalkieList = new List<TalkieObject>();
        globalTalkieList.AddRange(talkieObjects);

        int i = 0;
        foreach (var talkie in globalTalkieList)
        {
            print (i + ": " + talkie.name);
            i++;
        }

        return globalTalkieList;
    }

    // finds correct sprite, else returns null
    private Sprite FindSprite(List<TalkieDatabaseEntry> list, int emotionNum, TalkieMouth mouth, TalkieEyes eyes, TalkieCharacter character, int segIndex)
    {
        foreach (var entry in list)
        {
            if (entry.emotionNum == emotionNum && entry.mouth == mouth && entry.eyes == eyes)
                return entry.sprite;
        }

        GameManager.instance.SendError(this, "could not find talkie sprite for " + character + ": emotion " + emotionNum + ", mouth " + mouth + ", eyes " + eyes + " in \'" + TalkieManager.instance.currentTalkie.name + "\' index: " + segIndex);

        // return default sprite (element 0 in list)
        return list[0].sprite;
    }

    public AudioClip GetTalkieReactionDuplicate(string str)
    {
        string lowercase_str = str.ToLower();
        switch (lowercase_str)
        {
            default: return null;
            case "marcus_laugh": return marcusLaughList[Random.Range(0, marcusLaughList.Count)];
            case "brutus_laugh": return brutusLaughList[Random.Range(0, brutusLaughList.Count)];
            case "brutus_heh": return brutusHehList[Random.Range(0, brutusHehList.Count)];
            case "marcus_argh": return marcusArghList[Random.Range(0, marcusArghList.Count)];
            case "marcus_grr": return marcusGrrList[Random.Range(0, marcusGrrList.Count)];

            case "redwally_gasp": return redWallyGaspList[Random.Range(0, redWallyGaspList.Count)];
            case "redwally_what": return redWallyWhatList[Random.Range(0, redWallyWhatList.Count)];
            case "redwally_huh": return redWallyHuhList[Random.Range(0, redWallyHuhList.Count)];
            case "redwally_oh": return redWallyOhList[Random.Range(0, redWallyOhList.Count)];
            case "redwally_darwin": return redWallyDarwinList[Random.Range(0, redWallyDarwinList.Count)];

            case "julius_haha": return juliusHahaList[Random.Range(0, juliusHahaList.Count)];
            case "julius_ahhah": return juliusAhHahList[Random.Range(0, juliusAhHahList.Count)];
            case "julius_hrm": return juliusHrmList[Random.Range(0, juliusHrmList.Count)];
            case "julius_ugh": return juliusUghList[Random.Range(0, juliusUghList.Count)];
            case "julius_grr": return juliusGrrList[Random.Range(0, juliusGrrList.Count)];

            case "red_gasp": return redGaspList[Random.Range(0, redGaspList.Count)];
            case "red_woohoo": return redWoohooList[Random.Range(0, redWoohooList.Count)];
            case "red_hurrah": return redHurrahList[Random.Range(0, redHurrahList.Count)];
            case "red_uhhuh": return redUhHuhList[Random.Range(0, redUhHuhList.Count)];
        }
    }

    public Sprite GetTalkieSprite(TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes, int segmentIndex)
    {
        switch (character)
        {
            default:
            case TalkieCharacter.None:
                return null;
            case TalkieCharacter.Red:
                return FindSprite(redSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Darwin:
                return FindSprite(darwinSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Wally:
                return FindSprite(wallySprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Julius:
                return FindSprite(juliusSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Marcus:
                return FindSprite(marcusSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Brutus:
                return FindSprite(brutusSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Lester:
                return FindSprite(lesterSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Clogg:
                return FindSprite(cloggSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Bubbles:
                return FindSprite(bubblesSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Ollie:
                return FindSprite(ollieSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Spindle:
                return FindSprite(spindleSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Sylvie:
                return FindSprite(sylvieSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Celeste:
                return FindSprite(celesteSprites, emotionNum, mouth, eyes, character, segmentIndex);
            case TalkieCharacter.Taxi:
                return FindSprite(taxiSprites, emotionNum, mouth, eyes, character, segmentIndex);
        }
    }

#if UNITY_EDITOR
    public void UpdateCreateObject(TalkieObject talkie)
    {
        string[] results = AssetDatabase.FindAssets(talkie.talkieName);
        TalkieObject yourObject = null;
        string result = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == talkie.talkieName + ".asset")
            {
                result = path;
                Debug.Log("result: " + result);
            }
        }

        // create new word pair object
        if(result == "")
        {
            GameManager.instance.SendLog(this, "creating new talkie object -> " + talkie.talkieName);
            yourObject = ScriptableObject.CreateInstance<TalkieObject>();
            AssetDatabase.CreateAsset(yourObject, talkie_object_creation_folder + talkie.talkieName + ".asset");
        }
        // get word pair object
        else
        {
            GameManager.instance.SendLog(this, "found talkie object -> " + talkie.talkieName);
            string path = AssetDatabase.GUIDToAssetPath(results[0]);
            yourObject = (TalkieObject)AssetDatabase.LoadAssetAtPath(path, typeof(TalkieObject));
        }

        yourObject.talkieName = talkie.talkieName;
        yourObject.quipsCollection = talkie.quipsCollection;

        if (talkie.quipsCollection)
        {
            yourObject.validQuipIndexes = new List<int>();
            yourObject.validQuipIndexes.AddRange(talkie.validQuipIndexes);
        }

        yourObject.start = talkie.start;
        yourObject.addBackgroundBeforeTalkie = talkie.addBackgroundBeforeTalkie;
        yourObject.addLetterboxBeforeTalkie = talkie.addLetterboxBeforeTalkie;

        yourObject.ending = talkie.ending;
        yourObject.removeBackgroundAfterTalkie = talkie.removeBackgroundAfterTalkie;
        yourObject.removeLetterboxAfterTalkie = talkie.removeLetterboxAfterTalkie;

        yourObject.segmnets = talkie.segmnets;
        
        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}
