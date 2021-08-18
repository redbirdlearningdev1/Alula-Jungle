using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Talkie Objects")]          // Where in the code base is the talkie called from?
    public TalkieObject boatGame;       // script: NewBoatGameManager.cs
    public TalkieObject dock_1;         // script: ScrollMapManager.cs   
    public TalkieObject dock_2;         // script: ScrollMapManager.cs   

    public TalkieObject gorillaIntro_1; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_2; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_3; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_4; // script: MapCharacter.cs
    public TalkieObject gorillaIntro_5; // script: MapCharacter.cs

    public TalkieObject pre_minigame;   // script: MapIcon.cs
    public TalkieObject pre_darwin;     // script: MapCharacter.cs

    public TalkieObject red_notices_lester; // script: ScrollMapManager.cs
    public TalkieObject darwin_forces;      // script: ScrollMapManager.cs

    public TalkieObject lester_intro_1; // script: WagonWindowController.cs

    public TalkieObject villageRebuilt_1; // script: ScrollMapManager.cs
    public TalkieObject villageRebuilt_2; // script: ScrollMapManager.cs
    public TalkieObject villageRebuilt_3; // script: ScrollMapManager.cs

    public TalkieObject julius_challenges;
    public TalkieObject julius_wins;
    public TalkieObject julius_wins_again;

    public TalkieObject julius_loses__marcus_challenges;
    public TalkieObject marcus_challenges;
    public TalkieObject marcus_wins;
    public TalkieObject marcus_wins_again;

    public TalkieObject marcus_loses__brutus_challenges;
    public TalkieObject brutus_challenges;
    public TalkieObject brutus_wins;
    public TalkieObject brutus_wins_again;

    public TalkieObject challengeSignQuips;


    [Header("Talkie Objects")]
    public TalkieObject darwinQuips;    // script: MapCharacter.cs

    [Header("Character Sprites")]
    public List<TalkieDatabaseEntry> redSprites;
    public List<TalkieDatabaseEntry> darwinSprites;
    public List<TalkieDatabaseEntry> wallySprites;
    public List<TalkieDatabaseEntry> juliusSprites;
    public List<TalkieDatabaseEntry> marcusSprites;
    public List<TalkieDatabaseEntry> brutusSprites;
    public List<TalkieDatabaseEntry> lesterSprites;

    private List<TalkieObject> globalTalkieList; // list of all talkies in this database

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

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
        }
    }
}
