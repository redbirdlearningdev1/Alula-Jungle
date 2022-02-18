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
    public Character character;
    public Animator mapAnimator;
    public Animator characterAnimator;

    [SerializeField] private GameObject exclamationMark;

    [Header("Game Data")]
    public GameType gameType;

    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;

    void Awake()
    {
        // remove exclamation mark by default + make not interactable
        exclamationMark.transform.localScale = Vector3.zero;
        interactable = false;
    }

    public void ShowExclamationMark(bool opt)
    {
        if (opt)
        {
            exclamationMark.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
        else
        {
            exclamationMark.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
        
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
        print ("story beat: " + StudentInfoSystem.GetCurrentProfile().currStoryBeat);

        // check for character quips
        if (character == Character.Darwin)
        {
            if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.RedShowsStickerButton ||
                StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.VillageRebuilt ||
                StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForestPlayGames ||
                StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.WindyCliffPlayGames)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.DarwinQuips_1_p1);
                yield break;
            }

            else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.MermaidBeachPlayGames ||
                     StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ExitJunglePlayGames)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.DarwinQuips_1_p2);
                yield break;
            }

            else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaStudyPlayGames ||
                     StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_1 ||
                     StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_2 ||
                     StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_3)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.DarwinQuips_1_p3);
                yield break;
            }
        }
        else if (character == Character.Clogg)
        {
            if (StudentInfoSystem.GetCurrentProfile().currStoryBeat != StoryBeat.OrcVillageMeetClogg &&
                StudentInfoSystem.GetCurrentProfile().currStoryBeat != StoryBeat.OrcCampUnlocked)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CloggQuips_1_p1);
                yield break;
            }
        }

        bool playingChallengeGame = false;

        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillageIntro)
        {
            // play gorilla village intro sequence
            MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaVillageIntro);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

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
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PreStory_2_p1);
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
                MapAnimationController.instance.clogg.ShowExclamationMark(false);

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageIntro_2_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // unlock orc village
                ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.OrcVillage);
                ScrollMapManager.instance.UpdateMapIcons(true);
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
                // play SF unlocked
                MapAnimationController.instance.PlayMapAnim(MapAnim.SpookyForestIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

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
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PreStory_2_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcCampUnlocked)
        {
            // only continue if tapped on clogg
            if (character == Character.Clogg)
            {
                // remove exclamation mark
                MapAnimationController.instance.clogg.ShowExclamationMark(false);

                // add orc camp intro
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampIntro_1_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // unlock orc camp
                ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.OrcCamp);
                ScrollMapManager.instance.UpdateMapIcons(true);
                ScrollMapManager.instance.RevealStarsAtCurrentLocation();

                // save to SIS and exit to scroll map
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.WindyCliffUnlocked)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // play WC intro
                MapAnimationController.instance.PlayMapAnim(MapAnim.WindyCliffIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.FollowRedStoryGame)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // set the story game
                gameType = GameType.StoryGame;
                GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[2];

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PreStory_2_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.MermaidBeachUnlocked)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // play MB intro
                MapAnimationController.instance.PlayMapAnim(MapAnim.MermaidBeachIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.EmergingStoryGame)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // set the story game
                gameType = GameType.StoryGame;
                GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[3];

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PreStory_2_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ExitJungleUnlocked)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // play EJ intro
                MapAnimationController.instance.PlayMapAnim(MapAnim.ExitJungleIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ResolutionStoryGame)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // set the story game
                gameType = GameType.StoryGame;
                GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[4];

                // add pre story game talkie here
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PreStory_2_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaStudyUnlocked)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // play EJ intro
                MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaStudyIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PalaceIntro)
        {
            // only continue if tapped on gorilla
            if (character == Character.Darwin)
            {
                // play PI intro
                MapAnimationController.instance.PlayMapAnim(MapAnim.PalaceIntro);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                yield break;
            }
        }




        /* 
        ################################################
        #   CHALLENGE GAMES
        ################################################
        */  
        
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForest_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcCamp_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.WindyCliff_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PirateShip_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.MermaidBeach_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Ruins_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ExitJungle_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaStudy_challengeGame_1 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Monkeys_challengeGame_1)
        {
            if (character == Character.Julius)
            {
                // get current chapter
                Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

                // play correct RR talkie based on current chapter
                switch (currChapter)
                {
                    case Chapter.chapter_0:
                    case Chapter.chapter_1:
                    case Chapter.chapter_2:
                    case Chapter.chapter_3:
                        // play julius challenges
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_1_p1);
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;

                    case Chapter.chapter_4:
                        // play julius challenges
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_1_p2);
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;

                    case Chapter.chapter_5:
                        // play julius challenges
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_1_p3);
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;
                }

                // set bool to true
                playingChallengeGame = true;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForest_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcCamp_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.WindyCliff_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PirateShip_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.MermaidBeach_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Ruins_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ExitJungle_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaStudy_challengeGame_2 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Monkeys_challengeGame_2)
        {
            if (character == Character.Marcus)
            {

                // play marcus challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaMarcus_1_p1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
                
                // set bool to true
                playingChallengeGame = true;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Mudslide_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcVillage_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.SpookyForest_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.OrcCamp_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaPoop_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.WindyCliff_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PirateShip_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.MermaidBeach_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Ruins_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.ExitJungle_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaStudy_challengeGame_3 ||
                 StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.Monkeys_challengeGame_3)
        {
            if (character == Character.Brutus)
            {
                // play brutus challenges
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaBrutus_1_p1);
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

        // start game
        GoToGameDataSceneImmediately(playingChallengeGame);
    }

    public void GoToGameDataSceneImmediately(bool playingChallengeGame = false)
    {
        if (playingChallengeGame)
        {
            GameManager.instance.playingChallengeGame = true;
            SetGameManagerMapID(StudentInfoSystem.GetCurrentProfile().currStoryBeat);
        }
        
        GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(gameType), true, 0.5f, true);
    }

    private void SetGameManagerMapID(StoryBeat currStoryBeat)
    {
        switch (currStoryBeat)
        {
            // GV
            case StoryBeat.GorillaVillage_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.GV_challenge_1; return;
            case StoryBeat.GorillaVillage_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.GV_challenge_2; return;
            case StoryBeat.GorillaVillage_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.GV_challenge_3; return;

            // MS
            case StoryBeat.Mudslide_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.MS_challenge_1; return;
            case StoryBeat.Mudslide_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.MS_challenge_2; return;
            case StoryBeat.Mudslide_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.MS_challenge_3; return;

            // OV
            case StoryBeat.OrcVillage_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.OV_challenge_1; return;
            case StoryBeat.OrcVillage_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.OV_challenge_2; return;
            case StoryBeat.OrcVillage_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.OV_challenge_3; return;

            // SF
            case StoryBeat.SpookyForest_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.SF_challenge_1; return;
            case StoryBeat.SpookyForest_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.SF_challenge_2; return;
            case StoryBeat.SpookyForest_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.SF_challenge_3; return;

            // OC
            case StoryBeat.OrcCamp_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.OC_challenge_1; return;
            case StoryBeat.OrcCamp_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.OC_challenge_2; return;
            case StoryBeat.OrcCamp_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.OC_challenge_3; return;

            // GP
            case StoryBeat.GorillaPoop_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.GP_challenge_1; return;
            case StoryBeat.GorillaPoop_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.GP_challenge_2; return;
            case StoryBeat.GorillaPoop_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.GP_challenge_3; return;

            // WC
            case StoryBeat.WindyCliff_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.WC_challenge_1; return;
            case StoryBeat.WindyCliff_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.WC_challenge_2; return;
            case StoryBeat.WindyCliff_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.WC_challenge_3; return;

            // PS
            case StoryBeat.PirateShip_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.PS_challenge_1; return;
            case StoryBeat.PirateShip_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.PS_challenge_2; return;
            case StoryBeat.PirateShip_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.PS_challenge_3; return;

            // MB
            case StoryBeat.MermaidBeach_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.MB_challenge_1; return;
            case StoryBeat.MermaidBeach_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.MB_challenge_2; return;
            case StoryBeat.MermaidBeach_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.MB_challenge_3; return;

            // R
            case StoryBeat.Ruins_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.R_challenge_1; return;
            case StoryBeat.Ruins_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.R_challenge_2; return;
            case StoryBeat.Ruins_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.R_challenge_3; return;

            // EJ
            case StoryBeat.ExitJungle_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.EJ_challenge_1; return;
            case StoryBeat.ExitJungle_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.EJ_challenge_2; return;
            case StoryBeat.ExitJungle_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.EJ_challenge_3; return;

            // GS
            case StoryBeat.GorillaStudy_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.GS_challenge_1; return;
            case StoryBeat.GorillaStudy_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.GS_challenge_2; return;
            case StoryBeat.GorillaStudy_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.GS_challenge_3; return;

            // M
            case StoryBeat.Monkeys_challengeGame_1: GameManager.instance.mapID = MapIconIdentfier.M_challenge_1; return;
            case StoryBeat.Monkeys_challengeGame_2: GameManager.instance.mapID = MapIconIdentfier.M_challenge_2; return;
            case StoryBeat.Monkeys_challengeGame_3: GameManager.instance.mapID = MapIconIdentfier.M_challenge_3; return;
        }   
    }
}
