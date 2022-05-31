using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISystem
{
    private static List<GameType> minigameOptions;
    private static List<int> gameStars;


    public static GameType DetermineMinigame(StudentPlayerData playerData)
    {
        switch (playerData.minigamesPlayed)
        {
            case 0: return GameType.FroggerGame;
            case 1: return GameType.RummageGame;
            case 2: return GameType.SeashellGame;
            case 3: return GameType.SpiderwebGame;
            case 4: return GameType.TurntablesGame;
            case 5: return GameType.PirateGame;

            default:
                minigameOptions = new List<GameType>();
                gameStars = new List<int>();

                int addedFrog = 0;
                int addedSea = 0;
                int addedSpider = 0;
                int addedTurn = 0;
                int addedPirate = 0;
                int addedRummage = 0;

                gameStars.Add(playerData.starsFrogger);
                gameStars.Add(playerData.starsSeashell);
                gameStars.Add(playerData.starsSpiderweb);
                gameStars.Add(playerData.starsTurntables);
                gameStars.Add(playerData.starsPirate);
                gameStars.Add(playerData.starsRummage);
                gameStars.Sort();

                for (int i = 0; i < gameStars.Count; i++)
                {
                    if (playerData.starsFrogger == gameStars[i] && addedFrog == 0 && playerData.lastGamePlayed != GameType.FroggerGame)
                    {
                        minigameOptions.Add(GameType.FroggerGame);
                        addedFrog = 1;
                    }
                    if (playerData.starsSeashell == gameStars[i] && addedSea == 0 && playerData.lastGamePlayed != GameType.SeashellGame)
                    {
                        minigameOptions.Add(GameType.SeashellGame);
                        addedSea = 1;
                    }
                    if (playerData.starsSpiderweb == gameStars[i] && addedSpider == 0 && playerData.lastGamePlayed != GameType.SpiderwebGame)
                    {
                        minigameOptions.Add(GameType.SpiderwebGame);
                        addedSpider = 1;
                    }
                    if (playerData.starsTurntables == gameStars[i] && addedTurn == 0 && playerData.lastGamePlayed != GameType.TurntablesGame)
                    {
                        minigameOptions.Add(GameType.TurntablesGame);
                        addedTurn = 1;
                    }
                    if (playerData.starsPirate == gameStars[i] && addedPirate == 0 && playerData.lastGamePlayed != GameType.PirateGame)
                    {
                        minigameOptions.Add(GameType.PirateGame);
                        addedPirate = 1;
                    }
                    if (playerData.starsRummage == gameStars[i] && addedRummage == 0 && playerData.lastGamePlayed != GameType.RummageGame)
                    {
                        minigameOptions.Add(GameType.RummageGame);
                        addedRummage = 1;
                    }
                }

                /*if (minigameOptions.Count > 0)
                {
                    minigameOptions.Insert(0, minigameOptions[0]);
                }*/

                if (playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed <= 3)
                {
                    if (minigameOptions.Count > 0)
                    {
                        if (playerData.lastGamePlayed != minigameOptions[0])
                        {
                            return minigameOptions[0];
                        }
                        else
                        {
                            if (minigameOptions.Count > 1)
                            {
                                return minigameOptions[1];
                            }
                            else
                            {
                                Debug.LogError("Only 1 minigame option determined, and it was the same as the last game played");
                                return GameType.FroggerGame;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("No Minigame Option determined in DetermineMinigame()");
                        return GameType.FroggerGame;
                    }
                }
                else if (playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed >= 4 && playerData.rRumblePlayed == 1 && playerData.minigamesPlayed > 6)
                {
                    StudentInfoSystem.SaveStudentPlayerData();
                    if (minigameOptions.Count > 0)
                    {
                        if (playerData.lastGamePlayed != minigameOptions[0])
                        {
                            return minigameOptions[0];
                        }
                        else
                        {
                            if (minigameOptions.Count > 1)
                            {
                                return minigameOptions[1];
                            }
                            else
                            {
                                Debug.LogError("Only 1 minigame option determined, and it was the same as the last game played");
                                return GameType.FroggerGame;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("No Minigame Option determined in DetermineMinigame()");
                        return GameType.FroggerGame;
                    }
                    //DetermineRoyalRumbleGame(playerData);
                }
                else
                {
                    StudentInfoSystem.SaveStudentPlayerData();
                    if (minigameOptions.Count > 0)
                    {
                        return minigameOptions[Random.Range(0, minigameOptions.Count)];
                    }
                    else
                    {
                        Debug.LogError("No Minigame Option determined in DetermineMinigame()");
                        return GameType.FroggerGame;
                    }

                }
        }
    }

    private static void CreateMinigameList()
    {
        // create minigame options list
        minigameOptions = new List<GameType>();
        minigameOptions.Add(GameType.FroggerGame);
        minigameOptions.Add(GameType.TurntablesGame);
        minigameOptions.Add(GameType.RummageGame);
        minigameOptions.Add(GameType.PirateGame);
        minigameOptions.Add(GameType.SpiderwebGame);
        minigameOptions.Add(GameType.SeashellGame);

        // remove last played game
        if (minigameOptions.Contains(GameManager.instance.prevGameTypePlayed))
            minigameOptions.Remove(GameManager.instance.prevGameTypePlayed);
    }

    public static GameType DetermineChallengeGame(MapLocation location)
    {
        // get student data
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.TigerPawPhotos);
        challengeGameOptions.Add(GameType.TigerPawCoins);


        if ((playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol) >= 9)
        {
            challengeGameOptions.Add(GameType.Password);
        }
        if ((playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol + playerData.starsPass) >= 18)
        {
            challengeGameOptions.Add(GameType.WordFactoryDeleting);
            challengeGameOptions.Add(GameType.WordFactorySubstituting);
            challengeGameOptions.Add(GameType.WordFactoryBuilding);
        }

        // remove options that are already used in this area
        switch (location)
        {
            case MapLocation.GorillaVillage:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType);
                break;

            case MapLocation.Mudslide:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType);
                break;

            case MapLocation.OrcVillage:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType);
                break;

            case MapLocation.SpookyForest:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType);
                break;

            case MapLocation.OrcCamp:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType);
                break;

            case MapLocation.GorillaPoop:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType);
                break;

            case MapLocation.WindyCliff:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType);
                break;

            case MapLocation.PirateShip:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType);
                break;

            case MapLocation.MermaidBeach:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType);
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType);
                break;

            case MapLocation.ExitJungle:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType);
                break;

            case MapLocation.GorillaStudy:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType);
                break;

            case MapLocation.Monkeys:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType);
                break;
        }

        if (playerData.blendPlayed == 0 && challengeGameOptions.Count > 0)
        {
            return challengeGameOptions[0];
        }
        else if (playerData.tPawPolPlayed == 0 && challengeGameOptions.Count > 0)
        {
            return challengeGameOptions[0];
        }
        else if (playerData.tPawCoinPlayed == 0 && challengeGameOptions.Count > 0)
        {
            return challengeGameOptions[0];
        }
        else if (challengeGameOptions.Count > 0)
        {
            int index = Random.Range(0, challengeGameOptions.Count);
            return challengeGameOptions[index];
        }
        else
        {
            // return random index
            challengeGameOptions.Clear();
            challengeGameOptions.Add(GameType.WordFactoryBlending);
            challengeGameOptions.Add(GameType.TigerPawPhotos);
            challengeGameOptions.Add(GameType.TigerPawCoins);
            challengeGameOptions.Add(GameType.WordFactoryBuilding);
            challengeGameOptions.Add(GameType.WordFactorySubstituting);
            challengeGameOptions.Add(GameType.WordFactoryDeleting);
            challengeGameOptions.Add(GameType.WordFactoryBuilding);
            challengeGameOptions.Add(GameType.Password);

            Debug.Log("challenge game options: " + challengeGameOptions.Count);

            int index = Random.Range(0, challengeGameOptions.Count);
            return challengeGameOptions[index];
        }
    }

    public static bool DetermineRoyalRumble()
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        // player must have played all 6 minigames before RR
        if (playerData.minigamesPlayed < 6)
        {
            return false;
        }

        // return false if royal rumble already active
        if (playerData.rRumblePlayed == 1)
        {
            playerData.rRumblePlayed = 0;
            return true;
        }

        else if (playerData.rRumblePlayed == 0 && playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed >= 5)
        {
            playerData.rRumblePlayed = 1;
            return false;
        }
        else
        {
            return false;
        }
    }

    public static GameType DetermineRoyalRumbleGame()
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.TigerPawPhotos);
        challengeGameOptions.Add(GameType.TigerPawCoins);

        if ((playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol) >= 9)
        {
            challengeGameOptions.Add(GameType.Password);
        }
        if ((playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol + playerData.starsPass) >= 18)
        {
            challengeGameOptions.Add(GameType.WordFactoryDeleting);
            challengeGameOptions.Add(GameType.WordFactorySubstituting);
            challengeGameOptions.Add(GameType.WordFactoryBuilding);
        }

        if (playerData.blendPlayed == 0)
        {
            return GameType.WordFactoryBlending;
        }
        else if (playerData.tPawPolPlayed == 0)
        {
            return GameType.TigerPawPhotos;
        }
        else if (playerData.tPawCoinPlayed == 0)
        {
            return GameType.TigerPawCoins;
        }
        else
        {
            
            Dictionary<GameType, int> challengeGameStars = new Dictionary<GameType, int>();
            challengeGameStars.Add(GameType.WordFactoryBlending, playerData.starsBlend);
            challengeGameStars.Add(GameType.TigerPawPhotos, playerData.starsTPawPol);
            challengeGameStars.Add(GameType.TigerPawCoins, playerData.starsTPawCoin);
            challengeGameStars.Add(GameType.Password, playerData.starsPass);
            challengeGameStars.Add(GameType.WordFactoryDeleting, playerData.starsDel);
            challengeGameStars.Add(GameType.WordFactorySubstituting, playerData.starsSub);
            challengeGameStars.Add(GameType.WordFactoryBuilding, playerData.starsBuild);
            
            KeyValuePair<GameType, int> worstGame = new KeyValuePair<GameType, int>(GameType.WordFactoryBlending, playerData.starsBlend);
            foreach (KeyValuePair<GameType, int> pair in challengeGameStars)
            {
                if (worstGame.Value > pair.Value)
                {
                    worstGame = pair;
                }
            }

            return worstGame.Key;

            /*
            // return random index
            if (challengeGameOptions.Count > 0)
            {
                int index = Random.Range(0, challengeGameOptions.Count);
                return challengeGameOptions[index];
            }
            else
            {
                return GameType.WordFactoryBlending;
            }*/
        }
    }

    public static List<ChallengeWord> ChallengeWordSelectionBlending(List<ChallengeWord> excludeWords = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ChallengeWord> globalWordList = new List<ChallengeWord>();
        List<ChallengeWord> allGlobalWordList = new List<ChallengeWord>();
        List<ChallengeWord> unusedWordList = new List<ChallengeWord>();
        List<ChallengeWord> usedWordList = new List<ChallengeWord>();
        List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();
        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            globalWordList.Clear();
            globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
            unusedWordList.AddRange(globalWordList);
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                unusedWordList.AddRange(globalWordList);
            }

        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set3));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                    unusedWordList.AddRange(globalWordList);
                }
            }
        }
        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set4));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else if (random == 1)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                    unusedWordList.AddRange(globalWordList);
                }
                else
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set3));
                    unusedWordList.AddRange(globalWordList);
                }
            }
        }
        else
        {
            globalWordList.Clear();
            globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
            unusedWordList.AddRange(globalWordList);
        }

        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }

        if (unusedWordList.Contains(playerData.lastWordFaced))
        {
            unusedWordList.Remove(playerData.lastWordFaced);
        }

        ChallengeWord word;
        int index;
        if (unusedWordList.Count > 0)
        {
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }
        else
        {
            unusedWordList.AddRange(globalWordList);
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }

        playerData.lastWordFaced = word;

        if (playerData.starsBlend < 9)
        {
            try
            {
                while (word.elkoninCount != 2)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                    unusedWordList.Remove(word);
                }
            }
            catch
            {
                // re-add words to list if size is reduced to 0
                if (unusedWordList.Count == 0)
                    unusedWordList.AddRange(globalWordList);

                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else if (playerData.starsBlend < 18)
        {
            try
            {
                while (word.elkoninCount != 3)
                {

                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                    unusedWordList.Remove(word);
                }
            }
            catch
            {
                // re-add words to list if size is reduced to 0
                if (unusedWordList.Count == 0)
                    unusedWordList.AddRange(globalWordList);

                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else if (playerData.starsBlend < 36)
        {
            try
            {
                while (word.elkoninCount < 4)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                    unusedWordList.Remove(word);
                }
            }
            catch
            {
                // re-add words to list if size is reduced to 0
                if (unusedWordList.Count == 0)
                    unusedWordList.AddRange(globalWordList);

                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else
        {
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }

        // make sure word is not being used
        allGlobalWordList.Remove(word);

        // re-add words to list if size is reduced to 0
        if (allGlobalWordList.Count == 0)
            allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

        CurrentChallengeList.Add(word);
        for (int i = 0; i < 2; i++)
        {
            index = Random.Range(0, allGlobalWordList.Count);
            ChallengeWord falseWord = allGlobalWordList[index];
            if (playerData.starsBlend < 9)
            {
                try
                {
                    while (falseWord.elkoninCount != 2)
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsBlend < 15)
            {
                try
                {
                    while (falseWord.elkoninCount != 2 && (word.elkoninList[0] != falseWord.elkoninList[0] || word.elkoninList[1] != falseWord.elkoninList[1]))
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsBlend < 18)
            {
                try
                {
                    while (falseWord.elkoninCount != 3)
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsBlend < 21)
            {
                try
                {
                    while (falseWord.elkoninCount != 3 && (word.elkoninList[0] != falseWord.elkoninList[0] || word.elkoninList[2] != falseWord.elkoninList[2]))
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsBlend < 24)
            {
                try
                {
                    while (falseWord.elkoninCount < 4)
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsBlend < 36)
            {
                try
                {
                    while ((falseWord.elkoninCount < 4) && (word.elkoninList[0] != falseWord.elkoninList[0] || word.elkoninList[word.elkoninCount] != falseWord.elkoninList[falseWord.elkoninCount]))
                    {
                        index = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[index];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    index = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[index];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else
            {
                index = Random.Range(0, allGlobalWordList.Count);
                falseWord = allGlobalWordList[index];
                allGlobalWordList.Remove(falseWord);

                // re-add words to list if size is reduced to 0
                if (allGlobalWordList.Count == 0)
                    allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
            }
            CurrentChallengeList.Add(falseWord);
            allGlobalWordList.Remove(falseWord);

            // re-add words to list if size is reduced to 0
            if (allGlobalWordList.Count == 0)
                allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
        }



        // remove exclude words if list is not null
        if (excludeWords != null)
        {
            foreach (ChallengeWord excludeWord in excludeWords)
            {
                if (CurrentChallengeList.Contains(excludeWord) && CurrentChallengeList.Count > 3)
                {
                    CurrentChallengeList.Remove(excludeWord);
                }
            }
        }

        return CurrentChallengeList;
    }

    public static ActionWordEnum TigerPawPhotosCoinSelection()
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        ActionWordEnum Selected;
        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            if (set1.Count > 0)
            {
                Selected = set1[Random.Range(0, set1.Count)];
            }
            else
            {
                Debug.LogError("Not enough action words in set 1");
                Selected = ActionWordEnum._blank;
            }
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2 && set2.Count > 0)
            {
                Selected = set2[Random.Range(0, set2.Count)];
            }
            else if (set1.Count > 0)
            {
                Selected = set1[Random.Range(0, set1.Count)];
            }
            else
            {
                Debug.LogError("Not enough action words in set 1");
                Selected = ActionWordEnum._blank;
            }
        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2 && set3.Count > 0)
            {
                Selected = set3[Random.Range(0, set3.Count)];
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0 && set2.Count > 0)
                {
                    Selected = set2[Random.Range(0, set2.Count)];
                }
                else if (set1.Count > 0)
                {
                    Selected = set1[Random.Range(0, set1.Count)];
                }
                else
                {
                    Debug.LogError("Not enough action words in set 1 or 2");
                    Selected = ActionWordEnum._blank;
                }
            }
        }

        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2 && set4.Count > 0)
            {
                Selected = set4[Random.Range(0, set4.Count)];
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0 && set2.Count > 0)
                {
                    Selected = set2[Random.Range(0, set2.Count)];
                }
                else if (random == 1 && set1.Count > 0)
                {
                    Selected = set1[Random.Range(0, set1.Count)];
                }
                else if (set3.Count > 0)
                {
                    Selected = set3[Random.Range(0, set3.Count)];
                }
                else
                {
                    Debug.LogError("Not enough action words in sets 1, 2, or 3");
                    Selected = ActionWordEnum._blank;
                }
            }
        }
        else
        {
            if (set5.Count > 0)
            {
                Selected = set5[Random.Range(0, set5.Count)];
            }
            else
            {
                Debug.LogError("Not enough action words in set 5");
                Selected = ActionWordEnum._blank;
            }
        }

        

        return Selected;
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawPol(ActionWordEnum coin, List<ChallengeWord> excludeWords = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ChallengeWord> allGlobalWordList = new List<ChallengeWord>();
        List<ChallengeWord> coinGlobalWordList = new List<ChallengeWord>();

        List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();

        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);

        set5.Remove(coin);

        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

        List<ActionWordEnum> coinList = new List<ActionWordEnum>();
        coinList.Add(coin);
        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));

        int index = Random.Range(0, coinGlobalWordList.Count);
        ChallengeWord word = coinGlobalWordList[index];
        for (int i = 0; i < 2; i++)
        {
            index = Random.Range(0, coinGlobalWordList.Count);
            word = coinGlobalWordList[index];
            if (playerData.starsTPawPol < 9)
            {
                try
                {
                    while (word.elkoninCount != 2)
                    {
                        int randIndex = Random.Range(0, coinGlobalWordList.Count);
                        word = coinGlobalWordList[randIndex];
                        coinGlobalWordList.Remove(word);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));

                    int randIndex = Random.Range(0, coinGlobalWordList.Count);
                    word = coinGlobalWordList[randIndex];
                    coinGlobalWordList.Remove(word);

                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));
                }
            }
            else if (playerData.starsTPawPol < 18)
            {
                try
                {
                    while (word.elkoninCount != 3)
                    {
                        int randIndex = Random.Range(0, coinGlobalWordList.Count);
                        word = coinGlobalWordList[randIndex];
                        coinGlobalWordList.Remove(word);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));

                    int randIndex = Random.Range(0, coinGlobalWordList.Count);
                    word = coinGlobalWordList[randIndex];
                    coinGlobalWordList.Remove(word);

                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));
                }
            }
            else if (playerData.starsTPawPol < 36)
            {
                try
                {
                    while (word.elkoninCount < 4)
                    {
                        int randIndex = Random.Range(0, coinGlobalWordList.Count);
                        word = coinGlobalWordList[randIndex];
                        coinGlobalWordList.Remove(word);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));

                    int randIndex = Random.Range(0, coinGlobalWordList.Count);
                    word = coinGlobalWordList[randIndex];
                    coinGlobalWordList.Remove(word);

                    // re-add words to list if size is reduced to 0
                    if (coinGlobalWordList.Count == 0)
                        coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));
                }
            }
            else
            {
                int randIndex = Random.Range(0, coinGlobalWordList.Count);
                word = coinGlobalWordList[randIndex];
                coinGlobalWordList.Remove(word);

                // re-add words to list if size is reduced to 0
                if (coinGlobalWordList.Count == 0)
                    coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));
            }

            coinGlobalWordList.Remove(word);

            // re-add words to list if size is reduced to 0
            if (coinGlobalWordList.Count == 0)
                coinGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(coinList));

            CurrentChallengeList.Add(word);
            index = Random.Range(0, coinGlobalWordList.Count);
            word = coinGlobalWordList[index];
        }


        for (int i = 0; i < 3; i++)
        {
            index = Random.Range(0, allGlobalWordList.Count);
            ChallengeWord falseWord = allGlobalWordList[index];
            if (playerData.starsTPawPol < 9)
            {
                try
                {
                    while (falseWord.elkoninCount != 2)
                    {

                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsTPawPol < 15)
            {
                try
                {
                    while (falseWord.elkoninCount != 2 && (CurrentChallengeList[0].elkoninList[0] == falseWord.elkoninList[0] || CurrentChallengeList[0].elkoninList[CurrentChallengeList[0].elkoninCount] == falseWord.elkoninList[falseWord.elkoninCount]))
                    {
                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsTPawPol < 18)
            {
                try
                {
                    while (falseWord.elkoninCount != 3)
                    {

                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsTPawPol < 21)
            {
                try
                {
                    while (falseWord.elkoninCount != 3 && (CurrentChallengeList[0].elkoninList[0] == falseWord.elkoninList[0] || CurrentChallengeList[0].elkoninList[CurrentChallengeList[0].elkoninCount] == falseWord.elkoninList[falseWord.elkoninCount]))
                    {
                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsTPawPol < 24)
            {
                try
                {
                    while (falseWord.elkoninCount < 4)
                    {
                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else if (playerData.starsTPawPol < 36)
            {
                try
                {
                    while (falseWord.elkoninCount < 4 && (CurrentChallengeList[0].elkoninList[0] == falseWord.elkoninList[0] || CurrentChallengeList[0].elkoninList[CurrentChallengeList[0].elkoninCount] == falseWord.elkoninList[falseWord.elkoninCount]))
                    {
                        int randIndex = Random.Range(0, allGlobalWordList.Count);
                        falseWord = allGlobalWordList[randIndex];
                        allGlobalWordList.Remove(falseWord);
                    }
                }
                catch
                {
                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));

                    int randIndex = Random.Range(0, allGlobalWordList.Count);
                    falseWord = allGlobalWordList[randIndex];
                    allGlobalWordList.Remove(falseWord);

                    // re-add words to list if size is reduced to 0
                    if (allGlobalWordList.Count == 0)
                        allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
                }
            }
            else
            {
                int randIndex = Random.Range(0, allGlobalWordList.Count);
                falseWord = allGlobalWordList[randIndex];
                allGlobalWordList.Remove(falseWord);

                // re-add words to list if size is reduced to 0
                if (allGlobalWordList.Count == 0)
                    allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
            }

            CurrentChallengeList.Add(falseWord);
            allGlobalWordList.Remove(falseWord);

            // re-add words to list if size is reduced to 0
            if (allGlobalWordList.Count == 0)
                allGlobalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
        }


        // remove exclude words if list is not null
        if (excludeWords != null)
        {
            foreach (ChallengeWord excludeWord in excludeWords)
            {
                if (CurrentChallengeList.Contains(excludeWord) && CurrentChallengeList.Count > 5)
                {
                    CurrentChallengeList.Remove(excludeWord);
                }
            }
        }

        return CurrentChallengeList;
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawCoin(List<ChallengeWord> excludeWords = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ChallengeWord> globalWordList = new List<ChallengeWord>();
        List<ChallengeWord> allGlobalWordList = new List<ChallengeWord>();
        List<ChallengeWord> unusedWordList = new List<ChallengeWord>();
        List<ChallengeWord> usedWordList = new List<ChallengeWord>();
        List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();
        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        allGlobalWordList = ChallengeWordDatabase.GetChallengeWords(set5);

        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            globalWordList.Clear();
            globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
            unusedWordList.AddRange(globalWordList);
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                unusedWordList.AddRange(globalWordList);
            }

        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set3));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                    unusedWordList.AddRange(globalWordList);
                }
            }
        }

        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set4));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else if (random == 1)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set1));
                    unusedWordList.AddRange(globalWordList);
                }
                else
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set3));
                    unusedWordList.AddRange(globalWordList);
                }
            }
        }
        else
        {
            globalWordList.Clear();
            globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set5));
            unusedWordList.AddRange(globalWordList);
        }

        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }


        if (unusedWordList.Contains(playerData.lastWordFaced))
        {
            unusedWordList.Remove(playerData.lastWordFaced);
        }

        int index;
        ChallengeWord word;
        if (unusedWordList.Count > 0)
        {
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }
        else
        {
            unusedWordList.AddRange(globalWordList);
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }

        playerData.lastWordFaced = word;
        if (playerData.starsTPawCoin < 9)
        {
            try
            {
                while (word.elkoninCount != 2)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else if (playerData.starsTPawCoin < 18)
        {
            try
            {
                while (word.elkoninCount != 3)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }

        else if (playerData.starsTPawCoin < 36)
        {
            try
            {
                while (word.elkoninCount < 4)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else
        {
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }

        // make sure word is not being used
        if (usedWordList.Contains(word))
        {
            unusedWordList.Remove(word);
        }

        allGlobalWordList.Remove(word);
        unusedWordList.Remove(word);
        CurrentChallengeList.Add(word);


        // remove exclude words if list is not null
        if (excludeWords != null)
        {
            foreach (ChallengeWord excludeWord in excludeWords)
            {
                if (CurrentChallengeList.Contains(excludeWord) && CurrentChallengeList.Count > 1)
                {
                    CurrentChallengeList.Remove(excludeWord);
                }
            }
        }

        return CurrentChallengeList;
    }


    public static List<ActionWordEnum> TigerPawCoinsCoinSelection(StudentPlayerData playerData, List<ChallengeWord> Pold)
    {
        List<ActionWordEnum> Selected = new List<ActionWordEnum>();
        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);

        Selected.Add(Pold[0].set);
        set1.Remove(Pold[0].set);
        set2.Remove(Pold[0].set);
        set3.Remove(Pold[0].set);
        set4.Remove(Pold[0].set);
        set5.Remove(Pold[0].set);
        for (int i = 0; i < 4; i++)
        {
            int EightyTwenty = Random.Range(1, 11);
            if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
            {
                if (set1.Count > 0)
                {
                    int randSet1 = Random.Range(0, set1.Count);
                    Selected.Add(set1[randSet1]);
                    set1.RemoveAt(randSet1);
                }
                else
                {
                    Debug.LogError("No more action words to add to selection");
                }
            }
            else if (playerData.currentChapter == Chapter.chapter_2)
            {
                if (EightyTwenty > 2 && set2.Count > 0)
                {
                    int randSet2 = Random.Range(0, set2.Count);
                    Selected.Add(set2[randSet2]);
                    set2.RemoveAt(randSet2);
                }
                else if (set1.Count > 0)
                {
                    int randSet1 = Random.Range(0, set1.Count);
                    Selected.Add(set1[randSet1]);
                    set1.RemoveAt(randSet1);
                }
                else
                {
                    Debug.LogError("No more action words to add to selection");
                }
            }
            else if (playerData.currentChapter == Chapter.chapter_3)
            {
                if (EightyTwenty > 2 && set3.Count > 0)
                {
                    int randSet3 = Random.Range(0, set3.Count);
                    Selected.Add(set3[randSet3]);
                    set3.RemoveAt(randSet3);
                }
                else
                {
                    int random = Random.Range(0, 2);
                    if (random == 0 && set2.Count > 0)
                    {
                        int randSet2 = Random.Range(0, set2.Count);
                        Selected.Add(set2[randSet2]);
                        set2.RemoveAt(randSet2);
                    }
                    else if (set1.Count > 0)
                    {
                        int randSet1 = Random.Range(0, set1.Count);
                        Selected.Add(set1[randSet1]);
                        set1.RemoveAt(randSet1);
                    }
                    else
                    {
                        Debug.LogError("No more action words to add to selection");
                    }

                }

            }
            else if (playerData.currentChapter == Chapter.chapter_4)
            {
                if (EightyTwenty > 2 && set4.Count > 0)
                {
                    int randSet4 = Random.Range(0, set4.Count);
                    Selected.Add(set4[randSet4]);
                    set4.RemoveAt(randSet4);
                }
                else
                {
                    int random = Random.Range(0, 3);
                    if (random == 0 && set2.Count > 0)
                    {
                        int randSet2 = Random.Range(0, set2.Count);
                        Selected.Add(set2[randSet2]);
                        set2.RemoveAt(randSet2);
                    }
                    else if (random == 1 && set1.Count > 0)
                    {
                        int randSet1 = Random.Range(0, set1.Count);
                        Selected.Add(set1[randSet1]);
                        set1.RemoveAt(randSet1);
                    }
                    else if (set3.Count > 0)
                    {
                        int randSet3 = Random.Range(0, set3.Count);
                        Selected.Add(set3[randSet3]);
                        set3.RemoveAt(randSet3);
                    }
                    else
                    {
                        Debug.LogError("No more action words to add to selection");
                    }
                }

            }
            else
            {
                Selected.Add(set5[Random.Range(0, set5.Count)]);
            }
        }

        return Selected;
    }

    public static List<ChallengeWord> ChallengeWordPassword(List<ChallengeWord> excludeWords = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ChallengeWord> globalWordList = new List<ChallengeWord>();
        List<ChallengeWord> allGlobalWordList = new List<ChallengeWord>();
        List<ChallengeWord> unusedWordList = new List<ChallengeWord>();
        List<ChallengeWord> usedWordList = new List<ChallengeWord>();
        List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();
        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        allGlobalWordList = ChallengeWordDatabase.GetChallengeWords(set5);
        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            globalWordList = ChallengeWordDatabase.GetChallengeWords(set1);
            unusedWordList = globalWordList;
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2)
            {
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set2);
                unusedWordList = globalWordList;
            }
            else
            {
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set1);
                unusedWordList = globalWordList;
            }

        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2)
            {
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set3);
                unusedWordList = globalWordList;
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set2);
                    unusedWordList = globalWordList;
                }
                else
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set1);
                    unusedWordList = globalWordList;
                }
            }
        }
        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2)
            {
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set4);
                unusedWordList = globalWordList;
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set2);
                    unusedWordList = globalWordList;
                }
                else if (random == 1)
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set1);
                    unusedWordList = globalWordList;
                }
                else
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set3);
                    unusedWordList = globalWordList;
                }
            }
        }
        else
        {
            globalWordList = ChallengeWordDatabase.GetChallengeWords(set5);
            unusedWordList = globalWordList;
        }
        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];
        try
        {
            if (word == playerData.lastWordFaced)
            {
                return ChallengeWordPassword(excludeWords);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }
        playerData.lastWordFaced = word;
        if (playerData.starsPass < 9)
        {
            try
            {
                while (word.elkoninCount != 2)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else if (playerData.starsPass < 18)
        {
            try
            {
                while (word.elkoninCount != 3)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }

        else if (playerData.starsPass < 36)
        {
            try
            {
                while (word.elkoninCount < 4)
                {
                    index = Random.Range(0, unusedWordList.Count);
                    word = unusedWordList[index];
                }
            }
            catch
            {
                index = Random.Range(0, unusedWordList.Count);
                word = unusedWordList[index];
            }
        }
        else
        {
            index = Random.Range(0, unusedWordList.Count);
            word = unusedWordList[index];
        }

        // make sure word is not being used
        if (usedWordList.Contains(word))
        {
            unusedWordList.Remove(word);
        }
        allGlobalWordList.Remove(word);
        unusedWordList.Remove(word);
        CurrentChallengeList.Add(word);

        // remove exclude words if list is not null
        if (excludeWords != null)
        {
            foreach (ChallengeWord excludeWord in excludeWords)
            {
                if (CurrentChallengeList.Contains(excludeWord) && CurrentChallengeList.Count > 1)
                {
                    CurrentChallengeList.Remove(excludeWord);
                }
            }
        }

        return CurrentChallengeList;
    }

    public static WordPair ChallengeWordBuildingDeleting(List<WordPair> excludePairs = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();
        List<WordPair> pairPool = new List<WordPair>();
        WordPair selectedPairPool;


        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
            }
            else
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
            }

        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set3));
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
                }
                else
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
                }
            }
        }
        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set4));
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
                }
                else if (random == 1)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
                }
                else
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set3));
                }
            }
        }
        else
        {
            pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
        }

        // remove exclude pairs if list is not null
        if (excludePairs != null)
        {
            foreach (WordPair excludePair in excludePairs)
            {
                if (pairPool.Contains(excludePair) && pairPool.Count > 1)
                {
                    pairPool.Remove(excludePair);
                }
            }
        }

        int index = Random.Range(0, pairPool.Count);
        selectedPairPool = pairPool[index];
        try
        {
            if (selectedPairPool == playerData.lastWordPairFaced)
            {
                ChallengeWordBuildingDeleting(excludePairs);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }
        playerData.lastWordPairFaced = selectedPairPool;
        if (playerData.starsBuild < 9)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount != 2)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];
                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else if (playerData.starsBuild < 18)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount != 3)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];
                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else if (playerData.starsBuild < 36)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount < 4)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];
                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else
        {
            pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
            index = Random.Range(0, pairPool.Count);
            selectedPairPool = pairPool[index];
        }

        return selectedPairPool;
    }

    public static WordPair ChallengeWordSub(List<WordPair> excludePairs = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ActionWordEnum> set1 = new List<ActionWordEnum>();
        List<ActionWordEnum> set2 = new List<ActionWordEnum>();
        List<ActionWordEnum> set3 = new List<ActionWordEnum>();
        List<ActionWordEnum> set4 = new List<ActionWordEnum>();
        List<ActionWordEnum> set5 = new List<ActionWordEnum>();
        List<WordPair> pairPool = new List<WordPair>();
        WordPair selectedPairPool;

        set1.Add(ActionWordEnum.mudslide);
        set1.Add(ActionWordEnum.listen);
        set1.Add(ActionWordEnum.poop);
        set1.Add(ActionWordEnum.orcs);
        set1.Add(ActionWordEnum.think);
        set1.Add(ActionWordEnum.explorer);

        set2.Add(ActionWordEnum.hello);
        set2.Add(ActionWordEnum.spider);
        set2.Add(ActionWordEnum.scared);
        set2.Add(ActionWordEnum.thatguy);

        set3.Add(ActionWordEnum.choice);
        set3.Add(ActionWordEnum.strongwind);
        set3.Add(ActionWordEnum.pirate);
        set3.Add(ActionWordEnum.gorilla);
        set3.Add(ActionWordEnum.sounds);
        set3.Add(ActionWordEnum.give);

        set4.Add(ActionWordEnum.backpack);
        set4.Add(ActionWordEnum.frustrating);
        set4.Add(ActionWordEnum.bumphead);
        set4.Add(ActionWordEnum.baby);

        set5.Add(ActionWordEnum.mudslide);
        set5.Add(ActionWordEnum.listen);
        set5.Add(ActionWordEnum.poop);
        set5.Add(ActionWordEnum.orcs);
        set5.Add(ActionWordEnum.think);
        set5.Add(ActionWordEnum.hello);
        set5.Add(ActionWordEnum.spider);
        set5.Add(ActionWordEnum.scared);
        set5.Add(ActionWordEnum.explorer);
        set5.Add(ActionWordEnum.thatguy);
        set5.Add(ActionWordEnum.choice);
        set5.Add(ActionWordEnum.strongwind);
        set5.Add(ActionWordEnum.pirate);
        set5.Add(ActionWordEnum.gorilla);
        set5.Add(ActionWordEnum.sounds);
        set5.Add(ActionWordEnum.give);
        set5.Add(ActionWordEnum.backpack);
        set5.Add(ActionWordEnum.frustrating);
        set5.Add(ActionWordEnum.bumphead);
        set5.Add(ActionWordEnum.baby);


        int EightyTwenty = Random.Range(1, 11);
        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        {
            pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
            }
            else
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
            }

        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set3));
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
                }
                else
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
                }

            }

        }

        else if (playerData.currentChapter == Chapter.chapter_4)
        {
            if (EightyTwenty > 2)
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set4));
            }
            else
            {
                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
                }
                else if (random == 1)
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
                }
                else
                {
                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set3));
                }
            }
        }
        else
        {
            pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
        }


        // remove exclude pairs if list is not null
        if (excludePairs != null)
        {
            foreach (WordPair excludePair in excludePairs)
            {
                if (pairPool.Contains(excludePair) && pairPool.Count > 1)
                {
                    pairPool.Remove(excludePair);
                }
            }
        }


        int index = Random.Range(0, pairPool.Count);
        selectedPairPool = pairPool[index];
        try
        {
            if (selectedPairPool == playerData.lastWordPairFaced)
            {
                ChallengeWordSub(excludePairs);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }
        playerData.lastWordPairFaced = selectedPairPool;
        if (playerData.starsBuild < 9)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount != 2)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];

                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else if (playerData.starsBuild < 18)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount != 3)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];
                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }

        else if (playerData.starsBuild < 36)
        {
            try
            {
                while (pairPool[index].word1.elkoninCount < 4)
                {
                    pairPool.RemoveAt(index);
                    index = Random.Range(0, pairPool.Count);
                    selectedPairPool = pairPool[index];
                }
            }
            catch
            {
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else
        {
            pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
            index = Random.Range(0, pairPool.Count);
            selectedPairPool = pairPool[index];
        }

        return selectedPairPool;
    }






    private static Queue<GameType> bossBattleGameQueue;

    // placing boss battle determination function here for noe :) will move l8r
    public static GameType DetermineBossBattleGame()
    {
        // get array from SIS
        GameType[] sisArray = (GameType[])StudentInfoSystem.GetCurrentProfile().bossBattleGameQueue.Clone();

        // populate array iff empty
        if (sisArray.Length == 0)
        {
            sisArray = new GameType[7];
            sisArray[0] = GameType.WordFactoryBlending;
            sisArray[1] = GameType.TigerPawPhotos;
            sisArray[2] = GameType.TigerPawCoins;
            sisArray[3] = GameType.WordFactoryBuilding;
            sisArray[4] = GameType.WordFactorySubstituting;
            sisArray[5] = GameType.WordFactoryDeleting;
            sisArray[6] = GameType.Password;

            // save to SIS
            StudentInfoSystem.GetCurrentProfile().bossBattleGameQueue = (GameType[])sisArray.Clone();
            StudentInfoSystem.SaveStudentPlayerData();
        }

        // load in queue from SIS array
        bossBattleGameQueue = new Queue<GameType>();
        foreach (GameType game in sisArray)
        {
            bossBattleGameQueue.Enqueue(game);
        }
        // get next game type
        GameType nextGame = bossBattleGameQueue.Dequeue();

        // save new queue to SIS
        StudentInfoSystem.GetCurrentProfile().bossBattleGameQueue = bossBattleGameQueue.ToArray();
        StudentInfoSystem.SaveStudentPlayerData();

        return nextGame;
    }
}
