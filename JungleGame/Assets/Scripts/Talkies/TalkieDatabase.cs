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

    [Header("Red Sprites")]
    public List<TalkieDatabaseEntry> redSprites;

    [Header("Darwin Sprites")]
    public List<TalkieDatabaseEntry> darwinSprites;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // finds correct sprite, else returns null
    private Sprite FindSprite(List<TalkieDatabaseEntry> list, int emotionNum, TalkieMouth mouth, TalkieEyes eyes)
    {
        foreach (var entry in list)
        {
            if (entry.emotionNum == emotionNum && entry.mouth == mouth && entry.eyes == eyes)
                return entry.sprite;
        }

        GameManager.instance.SendError(this, "could not find talkie sprite");
        return null;
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
        }
    }
}
