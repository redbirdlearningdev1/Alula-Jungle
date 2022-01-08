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
    private const string talkie_object_folder = "Assets/Resources/TalkieObjects/";

    [Header("Boat Game")]          // Where in the code base is the talkie called from?
    public TalkieObject boatGame;       // script: NewBoatGameManager.cs
    public TalkieObject dock_1;         // script: ScrollMapManager.cs   
    public TalkieObject dock_2;         // script: ScrollMapManager.cs   

    [Header("Gorilla Intro")]
    public TalkieObject gorillaIntro_1; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_2; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_3; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_4; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_5; // script: MapCharacter.cs

    [Header("Pre Minigames")]
    public TalkieObject pre_minigame;   // script: MapIcon.cs
    public TalkieObject pre_darwin;     // script: MapCharacter.cs

    [Header("Pre Lester")]
    public TalkieObject red_notices_lester; // script: ScrollMapManager.cs
    public TalkieObject darwin_forces;      // script: ScrollMapManager.cs

    [Header("Pre Lester")]
    public TalkieObject lester_intro_1; // script: WagonWindowController.cs
    public TalkieObject lester_intro_2; // script: StickerBoardController.cs
    public TalkieObject lester_intro_3; // script: StickerBoardController.cs
    public TalkieObject lester_intro_4; // script: StickerBoardController.cs

    [Header("Village Rebuilt")]
    public TalkieObject villageRebuilt_1; // script: ScrollMapManager.cs
    public TalkieObject villageRebuilt_2; // script: ScrollMapManager.cs
    public TalkieObject villageRebuilt_3; // script: ScrollMapManager.cs

    [Header("Julius Challenge GV")]
    public TalkieObject julius_challenges; // script: MapCharacter.cs
    public TalkieObject julius_wins;       // script: ScrollMapManager.cs
    public TalkieObject julius_wins_again; // script: ScrollMapManager.cs

    [Header("Marcus Challenge GV")]
    public TalkieObject julius_loses__marcus_challenges; // script: ScrollMapManager.cs
    public TalkieObject marcus_challenges; // script: MapCharacter.cs
    public TalkieObject marcus_wins;       // script: ScrollMapManager.cs
    public TalkieObject marcus_wins_again; // script: ScrollMapManager.cs

    [Header("Brutus Challenge GV")]
    public TalkieObject marcus_loses__brutus_challenges;// script: ScrollMapManager.cs
    public TalkieObject brutus_challenges; // script: MapCharacter.cs
    public TalkieObject brutus_wins;       // script: ScrollMapManager.cs
    public TalkieObject brutus_wins_again; // script: ScrollMapManager.cs

    public TalkieObject challengeSignQuips;

    [Header("Village Defeated")]
    public TalkieObject villageChallengeDefeated_1; // script: ScrollMapManager.cs
    public TalkieObject villageChallengeDefeated_2; // script: ScrollMapManager.cs
    public TalkieObject villageChallengeDefeated_3; // script: ScrollMapManager.cs

    [Header("Mudslide")]
    public TalkieObject mudslideIntro; // script: ScrollMapManager.cs
    public TalkieObject mudslideRebuilt_1; // script: ScrollMapManager.cs
    public TalkieObject mudslideRebuilt_2; // script: ScrollMapManager.cs
    public TalkieObject mudslideChallengeDefeated_1; // script: ScrollMapManager.cs
    public TalkieObject mudslideChallengeDefeated_2; // script: ScrollMapManager.cs
    public TalkieObject mudslideChallengeDefeated_3; // script: ScrollMapManager.cs

    [Header("Orc Village")]
    public TalkieObject orcVillageIntro_1; // script: ScrollMapManager.cs
    public TalkieObject orcVillageIntro_2; // script: ScrollMapManager.cs
    public TalkieObject orcVillageIntro_3; // script: MapCharacter.cs
    public TalkieObject orcVillageRebuilt_1; // script: ScrollMapManager.cs
    public TalkieObject orcVillageRebuilt_2; // script: ScrollMapManager.cs
    public TalkieObject orcVillageChallengeDefeated_1; // script: ScrollMapManager.cs
    public TalkieObject orcVillageChallengeDefeated_2; // script: ScrollMapManager.cs

    [Header("Spooky Forest")]
    public TalkieObject spiderIntro_1; // script: MapCharacter.cs
    public TalkieObject spiderIntro_2; // script: MapCharacter.cs
    public TalkieObject spiderIntro_3; // script: MapCharacter.cs
    public TalkieObject spiderIntro_4; // script: MapCharacter.cs
    public TalkieObject spiderIntro_5; // script: MapCharacter.cs
    public TalkieObject spiderIntro_6; // script: MapCharacter.cs
    public TalkieObject spookyForest_1; // script: ScrollMapManager.cs
    public TalkieObject spookyForest_2; // script: ScrollMapManager.cs
    public TalkieObject spookyForest_3; // script: ScrollMapManager.cs
    public TalkieObject spookyForestChallengeDefeated_1; // script: ScrollMapManager.cs
    public TalkieObject spookyForestChallengeDefeated_2; // script: ScrollMapManager.cs
    public TalkieObject spookyForestChallengeDefeated_3; // script: ScrollMapManager.cs

    [Header("Orc Camp")]
    public TalkieObject orcCampIntro_1; // script: MapCharacter.cs
    public TalkieObject orcCampRebuilt_1; // script: ScrollMapManager.cs
    public TalkieObject orcCampRebuilt_2; // script: ScrollMapManager.cs
    public TalkieObject orcCampChallengeDefeated_1; // script: ScrollMapManager.cs
    public TalkieObject orcCampChallengeDefeated_2; // script: ScrollMapManager.cs
    public TalkieObject orcCampChallengeDefeated_3; // script: ScrollMapManager.cs
    



    [Header("Royal Rumble")]
    public TalkieObject defaultRRTalkie; // MiniganeWheelController.cs

    [Header("Quips")]
    public TalkieObject darwinQuips;    // script: MapCharacter.cs
    public TalkieObject cloggQuips;    // script: MapCharacter.cs

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

    // TODO: update this when all talkies are in
    public List<TalkieObject> GetGlobalTalkieList()
    {
        globalTalkieList = new List<TalkieObject>();
        
        globalTalkieList.Add(boatGame);
        globalTalkieList.Add(dock_1);
        globalTalkieList.Add(dock_2);
        globalTalkieList.Add(gorillaIntro_1);
        globalTalkieList.Add(gorillaIntro_2);
        globalTalkieList.Add(gorillaIntro_3);
        globalTalkieList.Add(gorillaIntro_4);
        globalTalkieList.Add(gorillaIntro_5);
        globalTalkieList.Add(pre_minigame);
        globalTalkieList.Add(pre_darwin);
        globalTalkieList.Add(red_notices_lester);
        globalTalkieList.Add(darwin_forces);
        globalTalkieList.Add(lester_intro_1);
        globalTalkieList.Add(villageRebuilt_1);
        globalTalkieList.Add(villageRebuilt_2);
        globalTalkieList.Add(villageRebuilt_3);
        globalTalkieList.Add(darwinQuips);

        return globalTalkieList;
    }

    // finds correct sprite, else returns null
    private Sprite FindSprite(List<TalkieDatabaseEntry> list, int emotionNum, TalkieMouth mouth, TalkieEyes eyes)
    {
        foreach (var entry in list)
        {
            if (entry.emotionNum == emotionNum && entry.mouth == mouth && entry.eyes == eyes)
                return entry.sprite;
        }

        // return default sprite (element 0 in list)
        return list[0].sprite;

        // GameManager.instance.SendError(this, "could not find talkie sprite");
        // return null;
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

    public Sprite GetTalkieSprite(TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes)
    {
        switch (character)
        {
            default:
            case TalkieCharacter.None:
                return null;
            case TalkieCharacter.Red:
                return FindSprite(redSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Darwin:
                return FindSprite(darwinSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Wally:
                return FindSprite(wallySprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Julius:
                return FindSprite(juliusSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Marcus:
                return FindSprite(marcusSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Brutus:
                return FindSprite(brutusSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Lester:
                return FindSprite(lesterSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Clogg:
                return FindSprite(cloggSprites, emotionNum, mouth, eyes);
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
            AssetDatabase.CreateAsset(yourObject, talkie_object_folder + talkie.talkieName + ".asset");
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
