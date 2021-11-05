using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Character
{
    None, Darwin, Julius, Marcus, Brutus, Clogg
}

public class MapCharacter : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = true;
    [SerializeField] private GameObject exclamationMark;
    public Character character;

    [Header("Game Data")]
    public GameType gameType;

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

    public void FlipCharacterToLeft()
    {
        transform.localScale = new Vector3(1f, 1f, 1f); // flip to face left side
    }   

    public void FlipCharacterToRight()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f); // flip to face right side
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
            transform.localScale = new Vector3(pressedScaleChange * transform.localScale.x, pressedScaleChange, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            // make sure x direction is consistent
            if (transform.localScale.x < 0)
                transform.localScale = new Vector3(-1, 1f, 1f);
            else 
                transform.localScale = new Vector3(1, 1f, 1f);

            // check for story beat before going to game
            StartCoroutine(CheckForStoryBeatRoutine());
        }
    }

    private IEnumerator CheckForStoryBeatRoutine()
    {
        // check for character quips
        if (character == Character.Darwin)
        {
            if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.RedShowsStickerButton ||
                StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.VillageRebuilt ||
                StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForestPlayGames)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.darwinQuips);
                yield break;
            }
        }
        else if (character == Character.Clogg)
        {
            if (StudentInfoSystem.GetCurrentProfile().currStoryBeat > StoryBeat.OrcVillageMeetClogg)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.cloggQuips);
                yield break;
            }
        }

        print ("story beat: " + StudentInfoSystem.GetCurrentProfile().currStoryBeat);
        bool playingChallengeGame = false;

        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillageIntro)
        {
            // remove exclamation mark from gorilla
            ScrollMapManager.instance.gorilla.ShowExclamationMark(false);
            ScrollMapManager.instance.gorilla.interactable = false;

            // play gorilla intro 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            // play gorilla intro 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger and monkies walk in
            MapAnimationController.instance.TigerAndMonkiesWalkInGV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            MapAnimationController.instance.TigerDestroyVillage();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // turn gorilla to face right
            ScrollMapManager.instance.gorilla.FlipCharacterToRight();

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

            // play gorilla intro 4
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_4);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            // turn gorilla to face left
            ScrollMapManager.instance.gorilla.FlipCharacterToLeft();

            // play gorilla intro 5
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_5);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // make gorilla interactable
            ScrollMapManager.instance.gorilla.ShowExclamationMark(true);
            ScrollMapManager.instance.gorilla.interactable = true;
            
            ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.GorillaVillage);
            ScrollMapManager.instance.UpdateMapIcons();

            // save to SIS and exit to scroll map
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
            yield break;
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PrologueStoryGame)
        {  
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // set the story game
                gameType = GameType.StoryGame;
                GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[0];

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.pre_darwin);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillageMeetClogg)
        {
            // only continue if tapped on Clogg
            if (character == Character.Clogg)
            {
                // remove exclamation mark
                ScrollMapManager.instance.clogg.ShowExclamationMark(false);

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageIntro_3);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // unlock orc village
                ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.OrcVillage);
                ScrollMapManager.instance.UpdateMapIcons();
                ScrollMapManager.instance.RevealStarsAtCurrentLocation();

                // save to SIS and exit to scroll map
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForestUnlocked)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                ScrollMapManager.instance.gorilla.ShowExclamationMark(false);
                ScrollMapManager.instance.gorilla.interactable = false;

                // spider intro 1
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // tiger and monkies walk in
                MapAnimationController.instance.TigerAndMonkiesWalkInSF();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // spider intro 2
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_2);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // tiger destroy forest
                MapAnimationController.instance.TigerDestroyForest();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // spider intro 3
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_3);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // tiger run away
                MapAnimationController.instance.TigerRunAwayDefeatedSF();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // spider intro 4
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_4);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // tiger run away
                MapAnimationController.instance.MonkeyExitAnimationSF();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // spider intro 5
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_5);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // spider intro 6
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spiderIntro_6);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // save to SIS and exit to scroll map
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();

                // make darwin interactable
                ScrollMapManager.instance.gorilla.interactable = true;
                ScrollMapManager.instance.gorilla.ShowExclamationMark(true);

                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.BeginningStoryGame)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // set the story game
                gameType = GameType.StoryGame;
                GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[1];

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.pre_darwin);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }

        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_1)
        {
            if (character == Character.Julius)
            {
                // play julius challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_challenges);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
                
                // set bool to true
                playingChallengeGame = true;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_2)
        {
            if (character == Character.Marcus)
            {
                // play julius challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_challenges);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
                
                // set bool to true
                playingChallengeGame = true;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_3 )
        {
            if (character == Character.Brutus)
            {
                // play julius challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.brutus_challenges);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
                
                // set bool to true
                playingChallengeGame = true;
            }
        }

        // do not go to game if talkie manager says not to
        if (TalkieManager.instance.doNotContinueToGame)
        {
            TalkieManager.instance.doNotContinueToGame = false;
            yield break;
        }

        GameManager.instance.playingChallengeGame = playingChallengeGame;

        // start game
        GoToGameDataSceneImmediately();
    }

    public void GoToGameDataSceneImmediately()
    {
        GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(gameType), true, 0.5f, true);
    }
}
