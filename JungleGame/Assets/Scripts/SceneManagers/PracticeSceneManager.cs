using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum PracticeModeGame
{
    blending,
    phoneme_in_word,
    elkonin_boxes,
    phoneme_changing,
    phoneme_id,
    phoneme_practice
}

public class PracticeSceneManager : MonoBehaviour
{
    public static PracticeSceneManager instance;

    public LerpableObject backButton;
    public LerpableObject windowBG;
    public Image windowBGImage;

    public DefaultPracticeWindow defaultPracticeWindow;
    public PhonemeChangingWindow phonemeChangingWindow;
    public PhonemePracticeWindow phonemePracticeWindow;

    public List<LerpableObject> practiceButtons;

    [Header("Addressables")]
    Dictionary<int, AsyncOperationHandle<Sprite>> avatarHandles = new Dictionary<int, AsyncOperationHandle<Sprite>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // hide back button
        backButton.transform.localScale = Vector3.zero;

        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.SetImageAlpha(windowBGImage, 0f);

        // hide all buttons
        foreach (var b in practiceButtons)
        {
            b.transform.localScale = Vector3.zero;
        }
    }

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        // play song
        AudioManager.instance.PlaySong(AudioDatabase.instance.SplashScreenSong);

        // short delay before showing UI
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);

        // show all buttons
        foreach (var b in practiceButtons)
        {
            b.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.25f);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // show UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }
    public void LoadAvatar(Image imageToSet, int profileIndex)
    {
        StartCoroutine(LoadAvatarRoutine(imageToSet, profileIndex));
    }

    IEnumerator LoadAvatarRoutine(Image imageToSet, int profileIndex)
    {
        AsyncOperationHandle<Sprite> avatarHandle;
        if (avatarHandles.ContainsKey(profileIndex))
        {
            avatarHandle = avatarHandles[profileIndex];
        }
        else
        {
            avatarHandle = GameManager.instance.avatars[profileIndex].LoadAssetAsync<Sprite>();
            avatarHandles.Add(profileIndex, avatarHandle);
        }

        yield return avatarHandle;

        imageToSet.sprite = avatarHandle.Result;
    }

    
    public void UnloadAvatar(int profileIndex)
    {
        if (avatarHandles.ContainsKey(profileIndex))
        {
            AsyncOperationHandle<Sprite> avatarHandle = avatarHandles[profileIndex];
            Addressables.Release(avatarHandle);
            avatarHandles.Remove(profileIndex);
        }
    }

    public void UnloadAllAvatars()
    {
        foreach (KeyValuePair<int, AsyncOperationHandle<Sprite>> pair in avatarHandles)
        {
            AsyncOperationHandle<Sprite> avatarHandle = pair.Value;
            Addressables.Release(avatarHandle);
        }
        avatarHandles.Clear();
    }

    public void RemoveWindowBG()
    {
        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.LerpImageAlpha(windowBGImage, 0f, 0.5f);
    }

    public void OnBackButtonPressed()
    {
        // show all buttons
        foreach (var b in practiceButtons)
        {
            b.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
        
        UnloadAllAvatars();
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        GameManager.instance.RestartGame();
    }

    public void OnBlendingPressed()
    {
        practiceButtons[0].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.blending);
    }

    public void OnPhonemeInWordPressed()
    {
        practiceButtons[1].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.phoneme_in_word);
    }

    public void OnElkoninBoxesPressed()
    {
        practiceButtons[2].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.elkonin_boxes);
    }

    public void OnPhonemeIDPressed()
    {
        practiceButtons[4].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.phoneme_id);
    }

    public void OnPhonemeChangingPressed()
    {
        practiceButtons[3].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        phonemeChangingWindow.OpenWindow();
    }

    public void OnPhonemePracticePressed()
    {
        practiceButtons[5].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        phonemePracticeWindow.OpenWindow();
    }

    ////////////////////////////////////////////////////

    public void StartPractice(PracticeModeGame game, int difficulty, int games, List<ActionWordEnum> phonemes, bool add, bool sub, bool del, bool move, bool sound, bool icon)
    {   
        List<GameType> gameQueue = new List<GameType>();
        GameType prevGameType = GameType.None;

        // curate specific practice queue
        switch (game)
        {
            case PracticeModeGame.blending:
                for (int i = 0; i < games; i++)
                {
                    gameQueue.Add(GameType.WordFactoryBlending);
                }
                break;
            
            case PracticeModeGame.elkonin_boxes:
                for (int i = 0; i < games; i++)
                {
                    gameQueue.Add(GameType.Password);
                }
                break;

            case PracticeModeGame.phoneme_changing:
                for (int i = 0; i < games; i++)
                {
                    GameType newType = DetermineNextPhonemeChangingGame(prevGameType, add, sub, del);
                    prevGameType = newType;
                    gameQueue.Add(newType);
                }
                break;

            case PracticeModeGame.phoneme_id:
                for (int i = 0; i < games; i++)
                {
                    gameQueue.Add(GameType.TigerPawPhotos);
                }
                break;

            case PracticeModeGame.phoneme_in_word:
                for (int i = 0; i < games; i++)
                {
                    gameQueue.Add(GameType.TigerPawCoins);
                }
                break;

            case PracticeModeGame.phoneme_practice:
                for (int i = 0; i < games; i++)
                {
                    GameType newType = DetermineNextPhonemePracticeGame(prevGameType, move, sound, icon);
                    prevGameType = newType;
                    gameQueue.Add(newType);
                }
                break;
        }

        string listofgames = "";
        foreach (var g in gameQueue)
        {
            listofgames += g + ", ";
        }
        print ("starting practice mode: " + listofgames);

        UnloadAllAvatars();

        // start practice modes
        GameManager.instance.SetPracticeMode(gameQueue, difficulty, phonemes);
        GameManager.instance.ContinuePracticeMode();
    }

    private GameType DetermineNextPhonemeChangingGame(GameType prevGameType, bool add, bool sub, bool del)
    {
        List<GameType> possibleGameTypes = new List<GameType>();
        if (add) possibleGameTypes.Add(GameType.WordFactoryBuilding);
        if (sub) possibleGameTypes.Add(GameType.WordFactorySubstituting);
        if (del) possibleGameTypes.Add(GameType.WordFactoryDeleting);

        if (possibleGameTypes.Count > 1 && possibleGameTypes.Contains(prevGameType))
        {
            possibleGameTypes.Remove(prevGameType);
        }

        GameType returnThisGame = possibleGameTypes[Random.Range(0, possibleGameTypes.Count)];
        return returnThisGame;
    }

    private GameType DetermineNextPhonemePracticeGame(GameType prevGameType, bool move, bool sound, bool icon)
    {
        List<GameType> possibleGameTypes = new List<GameType>();
        if (move) 
        {
            possibleGameTypes.Add(GameType.FroggerGame);
            possibleGameTypes.Add(GameType.TurntablesGame);
        }
        if (sound) 
        {
            possibleGameTypes.Add(GameType.TurntablesGame);
            possibleGameTypes.Add(GameType.SeashellGame);
        }
        if (icon)
        {   
            possibleGameTypes.Add(GameType.PirateGame);
            possibleGameTypes.Add(GameType.SpiderwebGame);
        }

        if (possibleGameTypes.Count > 1 && possibleGameTypes.Contains(prevGameType))
        {
            possibleGameTypes.Remove(prevGameType);
        }

        GameType returnThisGame = possibleGameTypes[Random.Range(0, possibleGameTypes.Count)];
        return returnThisGame;
    }
}
