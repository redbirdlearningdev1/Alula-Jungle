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

    /* 
    ################################################
    #   TALKIE OBJECTS
    ################################################
    */

    public TalkieObject BBChallenge_1_p1;
    public TalkieObject BBChallenge_1_p2;
    public TalkieObject BBChallenge_1_p3;
    public TalkieObject BBLose_1_p1;
    public TalkieObject BBLose_2_p1;
    public TalkieObject BBQuip_1_p1;
    public TalkieObject BBQuip_1_p2;
    public TalkieObject BBQuip_1_p3;
    public TalkieObject BBQuip_2_p1;
    public TalkieObject BBQuip_2_p2;
    public TalkieObject BBQuip_2_p3;
    public TalkieObject BBQuip_3_p1;
    public TalkieObject BBQuip_3_p2;
    public TalkieObject BBQuip_3_p3;
    public TalkieObject BBStory_1_p1;
    public TalkieObject BBStory_2_p1;
    public TalkieObject BBTryAgain_1_p1;
    public TalkieObject BBTryAgain_1_p2;
    public TalkieObject BBWin_1_p1;
    public TalkieObject BoatGame_1_p1;
    public TalkieObject BoatGame_2_p1;
    public TalkieObject BoatGame_2_p2;
    public TalkieObject BoatGame_3_p1;
    public TalkieObject ChaBrutus_1_p1;
    public TalkieObject ChaBrutusWins_1_p1;
    public TalkieObject ChaBrutusWins_1_p2;
    public TalkieObject ChaJulius_1_p1;
    public TalkieObject ChaJulius_1_p2;
    public TalkieObject ChaJulius_1_p3;
    public TalkieObject ChaJulius_2_p1;
    public TalkieObject ChaJulius_2_p2;
    public TalkieObject ChaJulius_2_p3;
    public TalkieObject ChaJuliusWins_1_p1;
    public TalkieObject ChaJuliusWins_1_p2;
    public TalkieObject ChaJuliusWins_1_p3;
    public TalkieObject ChaJuliusWins_2_p1;
    public TalkieObject ChaMarcus_1_p1;
    public TalkieObject ChaMarcus_2_p1;
    public TalkieObject ChaMarcus_2_p2;
    public TalkieObject ChaMarcusWins_1_p1;
    public TalkieObject ChaMarcusWins_1_p2;
    public TalkieObject ChaSignPost_1_p1;
    public TalkieObject ChaSignPost_1_p2;
    public TalkieObject ChaSignPost_1_p3;
    public TalkieObject ChaSignPost_1_p4;
    public TalkieObject ChaSignPost_1_p5;
    public TalkieObject CliffDefeated_1_p1;
    public TalkieObject CliffDefeated_1_p2;
    public TalkieObject CliffDefeated_1_p3;
    public TalkieObject CliffIntro_1_p1;
    public TalkieObject CliffIntro_1_p2;
    public TalkieObject CliffIntro_1_p3;
    public TalkieObject CliffIntro_1_p4;
    public TalkieObject CliffIntro_1_p5;
    public TalkieObject CliffRebuilt_1_p1;
    public TalkieObject CliffRebuilt_1_p2;
    public TalkieObject CliffRebuilt_1_p3;
    public TalkieObject CloggQuips_1_p1;
    public TalkieObject DarwinQuips_1_p1;
    public TalkieObject DarwinQuips_1_p2;
    public TalkieObject DarwinQuips_1_p3;
    public TalkieObject Dock_1_p1;
    public TalkieObject Dock_1_p2;
    public TalkieObject End_1_p1;
    public TalkieObject End_1_p2;
    public TalkieObject End_1_p3;
    public TalkieObject ExitDefeated_1_p1;
    public TalkieObject ExitDefeated_1_p2;
    public TalkieObject ExitDefeated_1_p3;
    public TalkieObject ExitIntro_1_p1;
    public TalkieObject ExitIntro_1_p2;
    public TalkieObject ExitIntro_1_p3;
    public TalkieObject ExitIntro_1_p4;
    public TalkieObject ExitIntro_1_p5;
    public TalkieObject ExitRebuilt_1_p1;
    public TalkieObject ExitRebuilt_1_p2;
    public TalkieObject FinalBoss_1_p1;
    public TalkieObject FinalBoss_2_p1;
    public TalkieObject ForceLester_1_p1;
    public TalkieObject GCampDefeated_1_p1;
    public TalkieObject GCampDefeated_1_p2;
    public TalkieObject GCampDefeated_1_p3;
    public TalkieObject GCampIntro_1_p1;
    public TalkieObject GCampRebuilt_1_p1;
    public TalkieObject GCampRebuilt_1_p2;
    public TalkieObject GCampRebuilt_1_p3;
    public TalkieObject GorillaIntro_1_p1;
    public TalkieObject GorillaIntro_1_p2;
    public TalkieObject GorillaIntro_1_p3;
    public TalkieObject GorillaIntro_1_p4;
    public TalkieObject LesterBuy_1_p1;
    public TalkieObject LesterBuy_1_p2;
    public TalkieObject LesterBuy_1_p3;
    public TalkieObject LesterFull_1_p1;
    public TalkieObject LesterIntro_1_p1;
    public TalkieObject LesterIntro_1_p2;
    public TalkieObject LesterIntro_1_p3;
    public TalkieObject LesterIntro_1_p4;
    public TalkieObject MermaidDefeated_1_p1;
    public TalkieObject MermaidDefeated_1_p2;
    public TalkieObject MermaidDefeated_1_p3;
    public TalkieObject MermaidIntro_1_p1;
    public TalkieObject MermaidIntro_1_p2;
    public TalkieObject MermaidIntro_1_p3;
    public TalkieObject MermaidIntro_1_p4;
    public TalkieObject MermaidIntro_1_p5;
    public TalkieObject MermaidQuip_1_p1;
    public TalkieObject MermaidQuip_1_p2;
    public TalkieObject MermaidRebuilt_1_p1;
    public TalkieObject MermaidRebuilt_1_p2;
    public TalkieObject MermaidRebuilt_1_p3;
    public TalkieObject MonkeyDefeated_1_p1;
    public TalkieObject MonkeyDefeated_1_p2;
    public TalkieObject MonkeyIntro_1_p1;
    public TalkieObject MonkeyIntro_1_p2;
    public TalkieObject MonkeyIntro_1_p3;
    public TalkieObject MonkeyQuips_1_p1;
    public TalkieObject MonkeyRebuilt_1_p1;
    public TalkieObject MonkeyRebuilt_1_p2;
    public TalkieObject MudslideDefeated_1_p1;
    public TalkieObject MudslideDefeated_1_p2;
    public TalkieObject MudslideDefeated_1_p3;
    public TalkieObject MudslideIntro_1_p1;
    public TalkieObject MudslideRebuilt_1_p1;
    public TalkieObject MudslideRebuilt_1_p2;
    public TalkieObject OCampDefeated_1_p1;
    public TalkieObject OCampDefeated_1_p2;
    public TalkieObject OCampDefeated_1_p3;
    public TalkieObject OCampIntro_1_p1;
    public TalkieObject OCampRebuilt_1_p1;
    public TalkieObject OCampRebuilt_1_p2;
    public TalkieObject OctopusQuips_1_p1;
    public TalkieObject OctopusQuips_2_p1;
    public TalkieObject OllieQuips_1_p1;
    public TalkieObject OllieQuips_2_p1;
    public TalkieObject OVillageDefeated_1_p1;
    public TalkieObject OVillageDefeated_1_p2;
    public TalkieObject OVillageIntro_1_p1;
    public TalkieObject OVillageIntro_1_p2;
    public TalkieObject OVillageIntro_2_p1;
    public TalkieObject OVillageRebuilt_1_p1;
    public TalkieObject OVillageRebuilt_1_p2;
    public TalkieObject PirateDefeated_1_p1;
    public TalkieObject PirateDefeated_1_p2;
    public TalkieObject PirateDefeated_1_p3;
    public TalkieObject PirateIntro_1_p1;
    public TalkieObject PirateIntro_1_p2;
    public TalkieObject PirateRebuilt_1_p1;
    public TalkieObject PirateRebuilt_1_p2;
    public TalkieObject PoopDefeated_1_p1;
    public TalkieObject PoopDefeated_1_p2;
    public TalkieObject PoopDefeated_1_p3;
    public TalkieObject PoopIntro_1_p1;
    public TalkieObject PoopRebuilt_1_p1;
    public TalkieObject PoopRebuilt_1_p2;
    public TalkieObject PreStory_1_p1;
    public TalkieObject PreStory_2_p1;
    public TalkieObject RedLester_1_p1;
    public TalkieObject RRGuardsIntro_1_p1;
    public TalkieObject RRGuardsIntro_2_p1;
    public TalkieObject RRGuardsIntro_2_p2;
    public TalkieObject RRGuardsLost_1_p1;
    public TalkieObject RRGuardsWins_1_p1;
    public TalkieObject RRJuliusIntro_1_p1;
    public TalkieObject RRJuliusLost_1_p1;
    public TalkieObject RRJuliusWins_1_p1;
    public TalkieObject RuinsDefeated_1_p1;
    public TalkieObject RuinsDefeated_1_p2;
    public TalkieObject RuinsDefeated_1_p3;
    public TalkieObject RuinsIntro_1_p1;
    public TalkieObject RuinsRebuilt_1_p1;
    public TalkieObject RuinsRebuilt_1_p2;
    public TalkieObject SectionIntro_1_p1;
    public TalkieObject SpiderDefeated_1_p1;
    public TalkieObject SpiderDefeated_1_p2;
    public TalkieObject SpiderDefeated_1_p3;
    public TalkieObject SpiderIntro_1_p1;
    public TalkieObject SpiderIntro_1_p2;
    public TalkieObject SpiderIntro_1_p3;
    public TalkieObject SpiderIntro_1_p4;
    public TalkieObject SpiderIntro_1_p5;
    public TalkieObject SpiderIntro_1_p6;
    public TalkieObject SpiderRebuilt_1_p1;
    public TalkieObject SpiderRebuilt_1_p2;
    public TalkieObject SpiderRebuilt_1_p3;
    public TalkieObject SpindleQuips_1_p1;
    public TalkieObject SpindleQuips_2_p1;
    public TalkieObject Taxi_1_p1;
    public TalkieObject Taxi_2_p1;
    public TalkieObject VillageDefeated_1_p1;
    public TalkieObject VillageDefeated_1_p3;
    public TalkieObject VillageDefeated_1_p4;
    public TalkieObject VillageRebuilt_1_p1;
    public TalkieObject VillageRebuilt_1_p2;
    public TalkieObject VillageRebuilt_1_p3;




    /* 
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

    // TODO: update this when all talkies are in
    public List<TalkieObject> GetGlobalTalkieList()
    {
        globalTalkieList = new List<TalkieObject>();
        
        // globalTalkieList.Add(boatGame);
        // globalTalkieList.Add(dock_1);
        // globalTalkieList.Add(dock_2);
        // globalTalkieList.Add(gorillaIntro_1);
        // globalTalkieList.Add(gorillaIntro_2);
        // globalTalkieList.Add(gorillaIntro_3);
        // globalTalkieList.Add(gorillaIntro_4);
        // globalTalkieList.Add(gorillaIntro_5);
        // globalTalkieList.Add(pre_minigame);
        // globalTalkieList.Add(pre_darwin);
        // globalTalkieList.Add(red_notices_lester);
        // globalTalkieList.Add(darwin_forces);
        // globalTalkieList.Add(lester_intro_1);
        // globalTalkieList.Add(villageRebuilt_1);
        // globalTalkieList.Add(villageRebuilt_2);
        // globalTalkieList.Add(villageRebuilt_3);
        // globalTalkieList.Add(darwinQuips);

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
            case TalkieCharacter.Bubbles:
                return FindSprite(bubblesSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Ollie:
                return FindSprite(ollieSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Spindle:
                return FindSprite(spindleSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Sylvie:
                return FindSprite(sylvieSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Celeste:
                return FindSprite(celesteSprites, emotionNum, mouth, eyes);
            case TalkieCharacter.Taxi:
                return FindSprite(taxiSprites, emotionNum, mouth, eyes);
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
