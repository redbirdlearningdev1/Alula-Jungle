using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISystem
{
    private static List<GameType> minigameOptions; 
    private static List<float> gameRatio;
    
    public static GameType DetermineMinigame(StudentPlayerData playerData)
    {
        //Debug.Log("Last Game Played " + playerData.lastGamePlayed);
        //Debug.Log("Game Before Last Game Played " +playerData.gameBeforeLastPlayed);
        //Debug.Log("Stars Last Game Played " +playerData.starsLastGamePlayed);
        //Debug.Log("Stars Before Last Game Played " +playerData.starsGameBeforeLastPlayed);
        //Debug.Log("Total Number of Stars Frogger " + playerData.totalStarsFrogger);
        //Debug.Log("Total Number of Stars Sea "  + playerData.totalStarsSeashell);
        //Debug.Log("Total Number of Stars Spider " + playerData.totalStarsSpiderweb);
        //Debug.Log("Total Number of Stars Turn " + playerData.totalStarsTurntables);
        //Debug.Log("Total Number of Stars Pirate " + playerData.totalStarsPirate);
        //Debug.Log("Total Number of Stars Rummage " + playerData.totalStarsRummage);
        //Debug.Log("Number of Stars Frogger " + playerData.starsFrogger);
        //Debug.Log("Number of Stars Sea " + playerData.starsSeashell);
        //Debug.Log("Number of Stars Spider " + playerData.starsSpiderweb);
        //Debug.Log("Number of Stars Turn " + playerData.starsTurntables);
        //Debug.Log("Number of Stars Pirate " + playerData.starsPirate);
        //Debug.Log("Number of Stars Rummage " + playerData.starsRummage); 

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
                gameRatio = new List<float>();

                int addedFrog = 0;
                int addedSea = 0;
                int addedSpider = 0;
                int addedTurn = 0;
                int addedPirate = 0;
                int addedRummage = 0;



                float frogSuccessRatio = (float)playerData.starsFrogger/(float)playerData.totalStarsFrogger;
                float seaSuccessRatio = (float)playerData.starsSeashell/(float)playerData.totalStarsSeashell;
                float spiderSuccessRatio = (float)playerData.starsSpiderweb/(float)playerData.totalStarsSpiderweb;
                float turnSuccessRatio = (float)playerData.starsTurntables/(float)playerData.totalStarsTurntables;
                float pirateSuccessRatio = (float)playerData.starsPirate/(float)playerData.totalStarsPirate;
                float rummageSuccessRatio = (float)playerData.starsRummage/(float)playerData.totalStarsRummage;
                
                gameRatio.Add(frogSuccessRatio);
                gameRatio.Add(seaSuccessRatio);
                gameRatio.Add(spiderSuccessRatio);
                gameRatio.Add(turnSuccessRatio);
                gameRatio.Add(pirateSuccessRatio);
                
                gameRatio.Add(rummageSuccessRatio);
                gameRatio.Sort();

                Debug.Log(gameRatio[0]);
                Debug.Log(gameRatio[1]);
                Debug.Log(gameRatio[2]);
                Debug.Log(gameRatio[3]);
                Debug.Log(gameRatio[4]);
                Debug.Log(gameRatio[5]);

                for(int i = 0 ; i < gameRatio.Count ; i++)
                {
                    if(frogSuccessRatio == gameRatio[i] && addedFrog == 0 && playerData.lastGamePlayed != GameType.FroggerGame )
                    {
                        minigameOptions.Add(GameType.FroggerGame);
                        addedFrog = 1;
                    }
                    if(seaSuccessRatio == gameRatio[i] && addedSea == 0 && playerData.lastGamePlayed != GameType.SeashellGame )
                    {
                        minigameOptions.Add(GameType.SeashellGame);
                        addedSea = 1;
                    }
                    if(spiderSuccessRatio == gameRatio[i] && addedSpider == 0 && playerData.lastGamePlayed != GameType.SpiderwebGame )
                    {
                        minigameOptions.Add(GameType.SpiderwebGame);
                        addedSpider = 1;
                    }
                    if(turnSuccessRatio == gameRatio[i] && addedTurn == 0 && playerData.lastGamePlayed != GameType.TurntablesGame )
                    {
                        minigameOptions.Add(GameType.TurntablesGame);
                        addedTurn = 1;
                    }
                    if(pirateSuccessRatio == gameRatio[i] && addedPirate == 0 && playerData.lastGamePlayed != GameType.PirateGame )
                    {
                        minigameOptions.Add(GameType.PirateGame);
                        addedPirate = 1;
                    }
                    if(rummageSuccessRatio == gameRatio[i] && addedRummage == 0 && playerData.lastGamePlayed != GameType.RummageGame )
                    {
                        minigameOptions.Add(GameType.RummageGame);
                        addedRummage = 1;
                    }
                }
                
                minigameOptions.Insert(0,minigameOptions[0]);

                Debug.Log(minigameOptions[0]);
                Debug.Log(minigameOptions[1]);
                Debug.Log(minigameOptions[2]);
                Debug.Log(minigameOptions[3]);
                Debug.Log(minigameOptions[4]);
                Debug.Log(minigameOptions[5]);

                if(playerData.starsLastGamePlayed+playerData.starsGameBeforeLastPlayed == 2 || playerData.starsLastGamePlayed+playerData.starsGameBeforeLastPlayed == 3)
                {
                    if(playerData.lastGamePlayed != minigameOptions[5])
                    {
                        return minigameOptions[5];
                    }
                    else
                    {
                        return minigameOptions[4];
                    }
                }
                else if(playerData.starsLastGamePlayed+playerData.starsGameBeforeLastPlayed >= 5 && playerData.rRumblePlayed == 1 && playerData.minigamesPlayed > 6)
                {
                    Debug.Log("ROYALRUMBLEDONE");
                    StudentInfoSystem.SaveStudentPlayerData();
                    return minigameOptions[Random.Range(0, minigameOptions.Count)];
                    //DetermineRoyalRumbleGame(playerData);
                }
                else
                {
                    Debug.Log("ROYALRUMBLENOTDONE");
                    StudentInfoSystem.SaveStudentPlayerData();
                    return minigameOptions[Random.Range(0, minigameOptions.Count)];
                    
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
        else if (challengeGameOptions.Count > 0 && challengeGameOptions.Count > 0)
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
            challengeGameOptions.Add(GameType.Password);

            Debug.Log("challenge game options: " + challengeGameOptions.Count);

            int index = Random.Range(0, challengeGameOptions.Count);
            return challengeGameOptions[index];
        }
    }

    public static bool DetermineRoyalRumble(StudentPlayerData playerData)
    {
        Debug.Log("THIS IS THE RrumbePlayed");
        Debug.Log(playerData.rRumblePlayed);
        // player must have played all 6 minigames before RR
        if (playerData.minigamesPlayed < 6)
            return false;

        // return false if royal rumble already active
        if (playerData.rRumblePlayed == 1)
        {
            playerData.rRumblePlayed = 0;
            return true;
        }
        else if(playerData.rRumblePlayed == 0 && playerData.starsLastGamePlayed+playerData.starsGameBeforeLastPlayed >= 5) 
        {
            playerData.rRumblePlayed = 1;
            return false;
        }
        else
        {
            return false;
        }

        // determine royal rumble
        
    }

    public static GameType DetermineRoyalRumbleGame(StudentPlayerData playerData)
    {
        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.TigerPawPhotos);
        challengeGameOptions.Add(GameType.TigerPawCoins);

        if( (playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol) >= 9)
        {
            challengeGameOptions.Add(GameType.Password);
        }   
        if( (playerData.starsBlend + playerData.starsBuild + playerData.starsTPawCoin + playerData.starsTPawPol + playerData.starsPass) >= 18)
        { 
            challengeGameOptions.Add(GameType.WordFactoryDeleting);
            challengeGameOptions.Add(GameType.WordFactorySubstituting);
        } 
        

        if( playerData.blendPlayed == 0)
        {
             return challengeGameOptions[0];
        }
        else if( playerData.tPawPolPlayed == 0)
        {
             return challengeGameOptions[1];
        }
        else if( playerData.tPawCoinPlayed == 0)
        {
             return challengeGameOptions[2];
        }
        else
        {
            // return random index
            int index = Random.Range(0, challengeGameOptions.Count);
            return challengeGameOptions[index];
        }
    }

    public static List<ChallengeWord> ChallengeWordSelectionBlending(StudentPlayerData playerData)
    {
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
        

        int EightyTwenty = Random.Range(0, 10);
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
        else if(playerData.currentChapter == Chapter.chapter_3)
        { 
            if (EightyTwenty > 2)
            {
                globalWordList.Clear();
                globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set3));
                unusedWordList.AddRange(globalWordList);
            }
            else
            {
                int random = Random.Range(0,1);
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
                int random = Random.Range(0,2);
                if (random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else if(random == 1)
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

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];
        try
        {
            if (word == playerData.lastWordFaced)
            {
                return ChallengeWordSelectionBlending(playerData);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
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
                    while (falseWord.elkoninCount != 2 && (word.elkoninList[0] != falseWord.elkoninList[0] || word.elkoninList[1] != falseWord.elkoninList[1])  )
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
            else if(playerData.starsBlend < 36)
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
        

        int EightyTwenty = Random.Range(0, 10);
        if (playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        { 
            Selected = set1[Random.Range(0,set1.Count)];
        }
        else if (playerData.currentChapter == Chapter.chapter_2 )
        { 
            if (EightyTwenty > 2)
            {
                Selected = set2[Random.Range(0,set2.Count)];
            }
            else
            {
                Selected = set1[Random.Range(0,set1.Count)];
            }
            
        }
        else if (playerData.currentChapter == Chapter.chapter_3 )
        { 
            if (EightyTwenty > 2)
            {
            
                Selected = set3[Random.Range(0,set3.Count)];
            }
            else
            {
                int random = Random.Range(0,1);
                if (random == 0)
                {
                    Selected = set2[Random.Range(0,set2.Count)];
                }
                else
                {
                    Selected = set1[Random.Range(0,set1.Count)];
                }

            }
            
        }

        else if (playerData.currentChapter == Chapter.chapter_4 )
        { 
            if (EightyTwenty > 2)
            {
            
                Selected = set4[Random.Range(0,set4.Count)];
            }
            else
            {
                int random = Random.Range(0,2);
                if (random == 0)
                {
                    Selected = set2[Random.Range(0,set2.Count)];
                }
                else if (random == 1)
                {
                    Selected = set1[Random.Range(0,set1.Count)];
                }
                else
                {
                    Selected = set3[Random.Range(0,set3.Count)];
                }

            }
            
        }
        else
        {
            Selected = set5[Random.Range(0,set5.Count)];
        }

        return Selected;
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawPol(ActionWordEnum coin)
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
            else if(playerData.starsTPawPol < 18)
            {
                try
                {
                    while(word.elkoninCount != 3)
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
                    while(word.elkoninCount < 4)
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
     
        return CurrentChallengeList;
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawCoin(StudentPlayerData playerData)
    {
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
        

        int EightyTwenty = Random.Range(0, 10);
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
                int random = Random.Range(0,1);
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
                int random = Random.Range(0,2);
                if(random == 0)
                {
                    globalWordList.Clear();
                    globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(set2));
                    unusedWordList.AddRange(globalWordList);
                }
                else if(random == 1)
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

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];
        try
        {
            if (word == playerData.lastWordFaced)
            {
                return ChallengeWordSelectionTigerPawCoin(playerData);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }

        playerData.lastWordFaced = word;
        if (playerData.starsTPawCoin  < 9)
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
        else if (playerData.starsTPawCoin  < 18)
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

        else if (playerData.starsTPawCoin  < 36)
        {
            try
            {
                while(word.elkoninCount < 4)
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

        return CurrentChallengeList;
    }
        
        
    public static List<ActionWordEnum> TigerPawCoinsCoinSelection(StudentPlayerData playerData, List<ChallengeWord> Pold )
    {
        List<ActionWordEnum> Selected = new List<ActionWordEnum>();;
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
        for(int i = 0; i < 4; i++)
        {
            int EightyTwenty = Random.Range(0, 10);
            if(playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
            { 

                int randSet1 = Random.Range(0,set1.Count);
                Selected.Add(set1[randSet1]);
                set1.RemoveAt(randSet1);
            }
            else if(playerData.currentChapter == Chapter.chapter_2 )
            { 
                if(EightyTwenty > 2)
                {
                    int randSet2 = Random.Range(0,set2.Count);
                    Selected.Add(set2[randSet2]);
                    set2.RemoveAt(randSet2);
                }
                else
                {
                    int randSet1 = Random.Range(0,set1.Count);
                    Selected.Add(set1[randSet1]);
                    set1.RemoveAt(randSet1);
                }
                
            }
            else if(playerData.currentChapter == Chapter.chapter_3 )
            { 
                if(EightyTwenty > 2)
                {
                
                    int randSet3 = Random.Range(0,set3.Count);
                    Selected.Add(set3[randSet3]);
                    set3.RemoveAt(randSet3);
                }
                else
                {
                    int random = Random.Range(0,1);
                    if(random == 0)
                    {
                        int randSet2 = Random.Range(0,set2.Count);
                        Selected.Add(set2[randSet2]);
                        set2.RemoveAt(randSet2);
                    }
                    else
                    {
                        int randSet1 = Random.Range(0,set1.Count);
                        Selected.Add(set1[randSet1]);
                        set1.RemoveAt(randSet1);
                    }

                }
                
            }

            else if(playerData.currentChapter == Chapter.chapter_4 )
            { 
                if(EightyTwenty > 2)
                {
                
                    int randSet4 = Random.Range(0,set4.Count);
                    Selected.Add(set4[randSet4]);
                    set4.RemoveAt(randSet4);
                }
                else
                {
                    int random = Random.Range(0,2);
                    if(random == 0)
                    {
                        int randSet2 = Random.Range(0,set2.Count);
                        Selected.Add(set2[randSet2]);
                        set2.RemoveAt(randSet2);
                    }
                    else if(random == 1)
                    {
                        int randSet1 = Random.Range(0,set1.Count);
                        Selected.Add(set1[randSet1]);
                        set1.RemoveAt(randSet1);
                    }
                    else
                    {
                        int randSet3 = Random.Range(0,set3.Count);
                        Selected.Add(set3[randSet3]);
                        set3.RemoveAt(randSet3);
                    }

                }
                
            }
            else
            {
                Selected.Add(set5[Random.Range(0,set5.Count)]);
            }
        }
        Debug.Log(Selected[0]);
        Debug.Log(Selected[1]);
        Debug.Log(Selected[2]);
        Debug.Log(Selected[3]);
        Debug.Log(Selected[4]);
        
        
        return Selected;
    }

    public static List<ChallengeWord> ChallengeWordPassword(StudentPlayerData playerData)
    {
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
        

        int EightyTwenty = Random.Range(0, 10);
        allGlobalWordList = ChallengeWordDatabase.GetChallengeWords(set5);
        if(playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        { 
            globalWordList = ChallengeWordDatabase.GetChallengeWords(set1);
            unusedWordList = globalWordList;
        }
        else if(playerData.currentChapter == Chapter.chapter_2 )
        { 
            if(EightyTwenty > 2)
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
        else if(playerData.currentChapter == Chapter.chapter_3 )
        { 
            if(EightyTwenty > 2)
            {
            
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set3);
                unusedWordList = globalWordList;
            }
            else
            {
                int random = Random.Range(0,1);
                if(random == 0)
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

        else if(playerData.currentChapter == Chapter.chapter_4 )
        { 
            if(EightyTwenty > 2)
            {
            
                globalWordList = ChallengeWordDatabase.GetChallengeWords(set4);
                unusedWordList = globalWordList;
            }
            else
            {
                int random = Random.Range(0,2);
                if(random == 0)
                {
                    globalWordList = ChallengeWordDatabase.GetChallengeWords(set2);
                    unusedWordList = globalWordList;
                }
                else if(random == 1)
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
        try{
            if(word == playerData.lastWordFaced)
            {
                return ChallengeWordPassword(playerData);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }   
        playerData.lastWordFaced = word; 
        if(playerData.starsPass  < 9)
        {
            try
            {
            while(word.elkoninCount != 2)
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
        else if(playerData.starsPass  < 18)
        {
            try
            {
            while(word.elkoninCount != 3)
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

        else if(playerData.starsPass  < 36)
        {
            try
            {

            while(word.elkoninCount < 4)
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

        return CurrentChallengeList;
        }

public static WordPair ChallengeWordBuildingDeleting(StudentPlayerData playerData)
    {
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
        

        int EightyTwenty = Random.Range(0, 10);
        if(playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        { 

            pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
        }
        else if(playerData.currentChapter == Chapter.chapter_2 )
        { 
            if(EightyTwenty > 2)
            {

                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
            }
            else
            {

                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
            }
            
        }
        else if(playerData.currentChapter == Chapter.chapter_3 )
        { 
            if(EightyTwenty > 2)
            {
            

                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set3));
            }
            else
            {
                int random = Random.Range(0,1);
                if(random == 0)
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
                }
                else
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set1));
                }

            }
            
        }

        else if(playerData.currentChapter == Chapter.chapter_4 )
        { 
            if(EightyTwenty > 2)
            {
            

                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set4));
            }
            else
            {
                int random = Random.Range(0,2);
                if(random == 0)
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set2));
                }
                else if(random == 1)
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



        int index = Random.Range(0, pairPool.Count);
        selectedPairPool = pairPool[index];
        try{
            if(selectedPairPool == playerData.lastWordPairFaced)
            {
                ChallengeWordBuildingDeleting(playerData);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }   
        playerData.lastWordPairFaced = selectedPairPool;
        if(playerData.starsBuild  < 9)
        {
            try{
            while(pairPool[index].word1.elkoninCount != 2)
            {
                
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
                
            }
            }
            catch{
                
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else if(playerData.starsBuild  < 18)
        {
            try{
            while(pairPool[index].word1.elkoninCount != 3 )
            {
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
            }
            catch{
                
                pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }

        else if(playerData.starsBuild  < 36)
        {
            try{
            while(pairPool[index].word1.elkoninCount < 4)
            {
                
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
            }
            catch{
                
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

public static WordPair ChallengeWordSub(StudentPlayerData playerData)
    {
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
        

        int EightyTwenty = Random.Range(0, 10);
        if(playerData.currentChapter == Chapter.chapter_0 || playerData.currentChapter == Chapter.chapter_1)
        { 

            pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
        }
        else if(playerData.currentChapter == Chapter.chapter_2 )
        { 
            if(EightyTwenty > 2)
            {

                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
            }
            else
            {

                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
            }
            
        }
        else if(playerData.currentChapter == Chapter.chapter_3 )
        { 
            if(EightyTwenty > 2)
            {
            

                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set3));
            }
            else
            {
                int random = Random.Range(0,1);
                if(random == 0)
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
                }
                else
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set1));
                }

            }
            
        }

        else if(playerData.currentChapter == Chapter.chapter_4 )
        { 
            if(EightyTwenty > 2)
            {
            

                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set4));
            }
            else
            {
                int random = Random.Range(0,2);
                if(random == 0)
                {

                    pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set2));
                }
                else if(random == 1)
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



        int index = Random.Range(0, pairPool.Count);
        selectedPairPool = pairPool[index];
        try{
            if(selectedPairPool == playerData.lastWordPairFaced)
            {
                ChallengeWordSub(playerData);
            }
        }
        catch
        {
            Debug.Log("This Broke maybe");
        }   
        playerData.lastWordPairFaced = selectedPairPool;
        if(playerData.starsBuild  < 9)
        {
            try{
            while(pairPool[index].word1.elkoninCount != 2)
            {
                
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
                
            }
            }
            catch{
                
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }
        else if(playerData.starsBuild  < 18)
        {
            try{
            while(pairPool[index].word1.elkoninCount != 3 )
            {
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
            }
            catch{
                Debug.Log("In catch");
                pairPool.AddRange(ChallengeWordDatabase.GetSubstitutionWordPairs(set5));
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
        }

        else if(playerData.starsBuild  < 36)
        {
            try{
            while(pairPool[index].word1.elkoninCount < 4)
            {
                Debug.Log("In hereere");
                pairPool.RemoveAt(index);
                index = Random.Range(0, pairPool.Count);
                selectedPairPool = pairPool[index];
            }
            }
            catch{
                Debug.Log("In catch");
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


    
}
