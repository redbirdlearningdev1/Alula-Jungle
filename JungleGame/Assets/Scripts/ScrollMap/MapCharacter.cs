using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Character
{
    None, Darwin, Julius, Marcus, Brutus
}

public class MapCharacter : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = true;
    [SerializeField] private GameObject exclamationMark;
    public Character character;

    [Header("Game Data")]
    public GameData gameData;

    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;

    void Awake()
    {
        // remove exclamation mark by default + make not interactable
        ShowExclamationMark(false);
        interactable = false;
    }

    public void ShowExclamationMark(bool opt)
    {
        exclamationMark.SetActive(opt);
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

        if (!isPressed)
        {
            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);

            // set prev map position
            ScrollMapManager.instance.SetPrevMapPos();

            // check for story beat before going to game
            StartCoroutine(CheckForStoryBeatRoutine());
        }
    }

    private IEnumerator CheckForStoryBeatRoutine()
    {
        // check for character quips
        if (character == Character.Darwin)
        {
            if (StudentInfoSystem.currentStudentPlayer.currStoryBeat > StoryBeat.PrologueStoryGame)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.darwinQuips);
                yield break;
            }
        }

        if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.GorillaVillageIntro)
        {
            // remove exclamation mark from gorilla
            ScrollMapManager.instance.gorilla.ShowExclamationMark(false);

            // play gorilla intro 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // play gorilla intro 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger and monkies walk in
            MapAnimationController.instance.TigerAndMonkiesWalkIn();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
            
            // turn gorilla to face right
            MapAnimationController.instance.gorilla.transform.localScale = new Vector3(-1, 1, 1);
            ScrollMapManager.instance.gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(2f);

            MapAnimationController.instance.TigerDestroyVillage();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
            
            // remove exclamation mark
            ScrollMapManager.instance.gorilla.ShowExclamationMark(false);

            // play gorilla intro 3
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            // tiger runs off screen
            MapAnimationController.instance.TigerRunAwayGV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            // monkies go hehe and haha then run off too
            MapAnimationController.instance.MonkeyExitAnimationGV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            // play gorilla intro 4
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_4);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            // turn gorilla to face left
            MapAnimationController.instance.gorilla.transform.localScale = new Vector3(1, 1, 1);

            // play gorilla intro 5
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_5);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            ScrollMapManager.instance.gorilla.ShowExclamationMark(true);

            // save to SIS and exit to scroll map
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();

            ScrollMapManager.instance.EnableAllMapIcons();
            yield break;
        }
        else if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.PrologueStoryGame)
        {  
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.pre_darwin);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.GorillaVillage_challengeGame_1)
        {
            if (character == Character.Julius)
            {
                // play julius challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_challenges);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }

        // do not go to game if talkie manager says not to
        if (TalkieManager.instance.doNotContinueToGame)
        {
            TalkieManager.instance.doNotContinueToGame = false;
            yield break;
        }

        // start game
        GoToGameDataSceneImmediately();
    }

    public void GoToGameDataSceneImmediately()
    {
        // go to correct game scene
        if (gameData)
        {
            GameManager.instance.SetData(gameData);
            GameManager.instance.LoadScene(gameData.sceneName, true);
        }
        else
        {
            GameManager.instance.LoadScene("MinigameDemoScene", true);
        }
    }
}
