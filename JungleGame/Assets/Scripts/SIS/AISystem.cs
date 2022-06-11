using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISystem
{
    private static List<GameType> minigameOptions;
    private static List<int> gamePlayedList;


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
                gamePlayedList = new List<int>();

                int addedFrog = 0;
                int addedSea = 0;
                int addedSpider = 0;
                int addedTurn = 0;
                int addedPirate = 0;
                int addedRummage = 0;

                gamePlayedList.Add(playerData.froggerPlayed);
                gamePlayedList.Add(playerData.seashellPlayed);
                gamePlayedList.Add(playerData.spiderwebPlayed);
                gamePlayedList.Add(playerData.turntablesPlayed);
                gamePlayedList.Add(playerData.piratePlayed);
                gamePlayedList.Add(playerData.rummagePlayed);
                gamePlayedList.Sort();

                for (int i = 0; i < gamePlayedList.Count; i++)
                {
                    if (playerData.froggerPlayed == gamePlayedList[i] && addedFrog == 0 && playerData.lastGamePlayed != GameType.FroggerGame)
                    {
                        minigameOptions.Add(GameType.FroggerGame);
                        addedFrog = 1;
                    }
                    if (playerData.seashellPlayed == gamePlayedList[i] && addedSea == 0 && playerData.lastGamePlayed != GameType.SeashellGame)
                    {
                        minigameOptions.Add(GameType.SeashellGame);
                        addedSea = 1;
                    }
                    if (playerData.spiderwebPlayed == gamePlayedList[i] && addedSpider == 0 && playerData.lastGamePlayed != GameType.SpiderwebGame)
                    {
                        minigameOptions.Add(GameType.SpiderwebGame);
                        addedSpider = 1;
                    }
                    if (playerData.turntablesPlayed == gamePlayedList[i] && addedTurn == 0 && playerData.lastGamePlayed != GameType.TurntablesGame)
                    {
                        minigameOptions.Add(GameType.TurntablesGame);
                        addedTurn = 1;
                    }
                    if (playerData.piratePlayed == gamePlayedList[i] && addedPirate == 0 && playerData.lastGamePlayed != GameType.PirateGame)
                    {
                        minigameOptions.Add(GameType.PirateGame);
                        addedPirate = 1;
                    }
                    if (playerData.rummagePlayed == gamePlayedList[i] && addedRummage == 0 && playerData.lastGamePlayed != GameType.RummageGame)
                    {
                        minigameOptions.Add(GameType.RummageGame);
                        addedRummage = 1;
                    }
                }

                /*if (minigameOptions.Count > 0)
                {
                    minigameOptions.Insert(0, minigameOptions[0]);
                }*/

                if (playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed <= 2)
                { // Player is doing poorly, so select an easy game from either Frogger or Rummage
                    int randNum = Random.Range(0, 2);

                    if (randNum == 0)
                    {
                        if (playerData.lastGamePlayed != GameType.FroggerGame)
                        {
                            return GameType.FroggerGame;
                        }
                        else
                        {
                            return GameType.RummageGame;
                        }
                    }
                    else
                    {
                        if (playerData.lastGamePlayed != GameType.RummageGame)
                        {
                            return GameType.RummageGame;
                        }
                        else
                        {
                            return GameType.FroggerGame;
                        }
                    }
                }
                else // if (playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed >= 4 && playerData.rRumblePlayed == 1 && playerData.minigamesPlayed > 6)
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

    public static List<ChallengeWord> ChallengeWordSelectionBlending(List<ChallengeWord> excludeWords = null, int difficultyLevel = -1, List<ActionWordEnum> phonemes = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        if (difficultyLevel == -1)
        {
            difficultyLevel = 1 + Mathf.FloorToInt(playerData.starsBlend / 3);
        }

        // Safety Checks, bound difficulty level between 1-6 only
        if (difficultyLevel < 1)
        {
            difficultyLevel = 1;
        }
        if (difficultyLevel > 6)
        {
            difficultyLevel = 6;
        }

        List<ActionWordEnum> set1 = new List<ActionWordEnum>()
        { ActionWordEnum.mudslide, ActionWordEnum.listen, ActionWordEnum.poop,
          ActionWordEnum.orcs, ActionWordEnum.think, ActionWordEnum.explorer };

        List<ActionWordEnum> set2 = new List<ActionWordEnum>()
        { ActionWordEnum.hello, ActionWordEnum.spider, ActionWordEnum.scared, ActionWordEnum.thatguy };

        List<ActionWordEnum> set3 = new List<ActionWordEnum>()
        { ActionWordEnum.choice, ActionWordEnum.strongwind, ActionWordEnum.pirate,
          ActionWordEnum.gorilla, ActionWordEnum.sounds, ActionWordEnum.give };

        List<ActionWordEnum> set4 = new List<ActionWordEnum>()
        { ActionWordEnum.backpack, ActionWordEnum.frustrating, ActionWordEnum.bumphead, ActionWordEnum.baby }; //, ActionWordEnum.hit };

        List<ActionWordEnum> prevPhonemes = new List<ActionWordEnum>();

        int elkoninValueMin;
        int elkoninValueMax;
        bool similarSounds;
        float currentSectionPercent = 1f;
        ChallengeWord correctWord;

        if (difficultyLevel % 2 == 1)
        { // Odd difficulty levels have no similar sounds
            similarSounds = false;
        }
        else
        { // Even difficulty levels have similar sounds
            similarSounds = true;
        }

        // Assign Elkonin Value based on the difficulty
        if (difficultyLevel <= 2)
        {
            elkoninValueMin = 2;
            elkoninValueMax = 3;
        }
        else if (difficultyLevel == 3)
        {
            elkoninValueMin = 3;
            elkoninValueMax = 3;
        }
        else if (difficultyLevel == 4)
        {
            elkoninValueMin = 3;
            elkoninValueMax = 4;
        }
        else
        {
            elkoninValueMin = 4;
            elkoninValueMax = 10;
        }

        // If phonemes weren't specified in the function parameters, assign phoneme groups and previous phoneme groups based on chapter
        // Additionally, assign the probability that the current phonemes get used over the previous phoneme group 0f - 1f
        if (phonemes == null)
        {
            prevPhonemes = new List<ActionWordEnum>();
            if (playerData.currentChapter <= Chapter.chapter_1)
            {
                phonemes.AddRange(set1);
                currentSectionPercent = 1f;
            }
            else if (playerData.currentChapter == Chapter.chapter_2)
            {
                prevPhonemes.AddRange(set1);
                phonemes.AddRange(set2);
                currentSectionPercent = 0.7f;
            }
            else if (playerData.currentChapter == Chapter.chapter_3)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                phonemes.AddRange(set3);
                currentSectionPercent = 0.6f;
            }
            else if (playerData.currentChapter == Chapter.chapter_4)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                prevPhonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 0.5f;
            }
            else
            {
                phonemes.AddRange(set1);
                phonemes.AddRange(set2);
                phonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 1f;
            }
        }

        // Get all challenge words for the main group of phonemes
        List<ChallengeWord> currSectionChallengeWords = new List<ChallengeWord>();
        currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        //Debug.Log("Getting Challenge Words for current group of phonemes: " + phonemes.Count);
        //Debug.Log("Challenge Words: " + currSectionChallengeWords.Count);

        // If we are in a section with a previous group of phonemes, get all challenge words for that group
        if (prevPhonemes.Count < 1)
        {
            prevPhonemes.AddRange(phonemes);
        }
        List<ChallengeWord> prevSectionChallengeWords = new List<ChallengeWord>();
        prevSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(prevPhonemes));
        //Debug.Log("Getting Challenge Words for previous group of phonemes: " + prevPhonemes.Count);
        //Debug.Log("Previous Challenge Words: " + prevSectionChallengeWords.Count);

        // Remove any words already used in this game
        foreach (ChallengeWord word in excludeWords)
        {
            if (currSectionChallengeWords.Contains(word))
            {
                currSectionChallengeWords.Remove(word);
            }

            if (prevSectionChallengeWords.Contains(word))
            {
                prevSectionChallengeWords.Remove(word);
            }
        }
        // If there aren't enough words to pick from, repopulate the list
        if (currSectionChallengeWords.Count < 1)
        {
            currSectionChallengeWords.Clear();
            currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        }
        if (prevSectionChallengeWords.Count < 1)
        {
            prevSectionChallengeWords.Clear();
            prevSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(prevPhonemes));
        }

        // Filtering out words with the incorrect elkonin values
        List<ChallengeWord> filteredCurrChallengeWords = new List<ChallengeWord>();
        List<ChallengeWord> filteredPrevChallengeWords = new List<ChallengeWord>();

        foreach (ChallengeWord word in currSectionChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredCurrChallengeWords.Add(word);
            }
        }
        foreach (ChallengeWord word in prevSectionChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredPrevChallengeWords.Add(word);
            }
        }

        if (filteredCurrChallengeWords.Count < 1)
        {
            filteredCurrChallengeWords.AddRange(currSectionChallengeWords);
        }
        if (filteredPrevChallengeWords.Count < 1)
        {
            filteredPrevChallengeWords.AddRange(prevSectionChallengeWords);
        }

        // Decide whether to use the current section or previous section for this selection
        float randNum = Random.Range(0f, 1f);
        if (randNum <= currentSectionPercent)
        {
            correctWord = filteredCurrChallengeWords[Random.Range(0, filteredCurrChallengeWords.Count)];
            filteredCurrChallengeWords.Remove(correctWord);
            if (filteredPrevChallengeWords.Contains(correctWord))
            {
                filteredPrevChallengeWords.Remove(correctWord);
            }
        }
        else
        {
            correctWord = filteredPrevChallengeWords[Random.Range(0, filteredPrevChallengeWords.Count)];
            filteredPrevChallengeWords.Remove(correctWord);
            if (filteredCurrChallengeWords.Contains(correctWord))
            {
                filteredCurrChallengeWords.Remove(correctWord);
            }
        }

        //Debug.Log("Current words after removing correct: " + filteredCurrChallengeWords.Count);
        //Debug.Log("Previous words after removing correct: " + filteredPrevChallengeWords.Count);

        if (filteredCurrChallengeWords.Count < 1)
        {
            filteredCurrChallengeWords.Clear();
            filteredCurrChallengeWords.AddRange(currSectionChallengeWords);
        }
        if (filteredPrevChallengeWords.Count < 1)
        {
            filteredPrevChallengeWords.Clear();
            filteredPrevChallengeWords.AddRange(currSectionChallengeWords);
        }

        List<ChallengeWord> wordsToReturn = new List<ChallengeWord>();
        wordsToReturn.Add(correctWord);

        List<ChallengeWord> finalCurrChallengeWords = new List<ChallengeWord>();
        finalCurrChallengeWords.AddRange(filteredCurrChallengeWords);
        List<ChallengeWord> finalPrevChallengeWords = new List<ChallengeWord>();
        finalPrevChallengeWords.AddRange(filteredPrevChallengeWords);

        // If we are supposed to, filter for only words with the same beginning or ending sound
        if (similarSounds)
        {
            foreach (ChallengeWord word in filteredCurrChallengeWords)
            {
                if (word.elkoninList[0] != correctWord.elkoninList[0] && word.elkoninList[word.elkoninList.Count - 1] != correctWord.elkoninList[correctWord.elkoninCount - 1])
                {
                    finalCurrChallengeWords.Remove(word);
                }
            }
            foreach (ChallengeWord word in filteredPrevChallengeWords)
            {
                if (word.elkoninList[0] != correctWord.elkoninList[0] && word.elkoninList[word.elkoninList.Count - 1] != correctWord.elkoninList[correctWord.elkoninCount - 1])
                {
                    finalPrevChallengeWords.Remove(word);
                }
            }

            //Debug.Log("Current count after removing non-similar words: " + finalCurrChallengeWords.Count);
            //Debug.Log("Previous count after removing non-similar words: " + finalPrevChallengeWords.Count);

            if (finalCurrChallengeWords.Count < 2)
            {
                finalCurrChallengeWords.AddRange(filteredCurrChallengeWords);
            }
            if (finalPrevChallengeWords.Count < 2)
            {
                finalPrevChallengeWords.AddRange(filteredPrevChallengeWords);
            }
        }

        //Debug.Log("Current Correct word: " + correctWord);

        // Repeat selection process for the two incorrect words
        for (int i = 0; i < 2; i++)
        {
            randNum = Random.Range(0f, 1f);
            if (randNum <= currentSectionPercent)
            {
                //Debug.Log("CurrCountBeforeRemove("+i+"): " + finalCurrChallengeWords.Count);
                int randIndex = Random.Range(0, finalCurrChallengeWords.Count);
                wordsToReturn.Add(finalCurrChallengeWords[randIndex]);
                if (finalPrevChallengeWords.Contains(finalCurrChallengeWords[randIndex]))
                {
                    finalPrevChallengeWords.Remove(finalCurrChallengeWords[randIndex]);
                }
                finalCurrChallengeWords.Remove(finalCurrChallengeWords[randIndex]);
                //Debug.Log("CurrCountAfterRemove("+i+"): " + finalCurrChallengeWords.Count);
            }
            else
            {
                int randIndex = Random.Range(0, finalPrevChallengeWords.Count);
                wordsToReturn.Add(finalPrevChallengeWords[randIndex]);
                if (finalCurrChallengeWords.Contains(finalPrevChallengeWords[randIndex]))
                {
                    finalCurrChallengeWords.Remove(finalPrevChallengeWords[randIndex]);
                }
                finalPrevChallengeWords.Remove(finalPrevChallengeWords[randIndex]);
            }
        }

        return wordsToReturn;
    }

    public static ActionWordEnum TigerPawPhotosCoinSelection(List<ActionWordEnum> phonemes = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        List<ActionWordEnum> set1 = new List<ActionWordEnum>()
        { ActionWordEnum.mudslide, ActionWordEnum.listen, ActionWordEnum.poop,
          ActionWordEnum.orcs, ActionWordEnum.think, ActionWordEnum.explorer };

        List<ActionWordEnum> set2 = new List<ActionWordEnum>()
        { ActionWordEnum.hello, ActionWordEnum.spider, ActionWordEnum.scared, ActionWordEnum.thatguy };

        List<ActionWordEnum> set3 = new List<ActionWordEnum>()
        { ActionWordEnum.choice, ActionWordEnum.strongwind, ActionWordEnum.pirate,
          ActionWordEnum.gorilla, ActionWordEnum.sounds, ActionWordEnum.give };

        List<ActionWordEnum> set4 = new List<ActionWordEnum>()
        { ActionWordEnum.backpack, ActionWordEnum.frustrating, ActionWordEnum.bumphead, ActionWordEnum.baby }; //, ActionWordEnum.hit };

        List<ActionWordEnum> prevPhonemes = new List<ActionWordEnum>();

        float currentSectionPercent = 1f;

        // If phonemes weren't specified in the function parameters, assign phoneme groups and previous phoneme groups based on chapter
        // Additionally, assign the probability that the current phonemes get used over the previous phoneme group 0f - 1f
        if (phonemes == null)
        {
            prevPhonemes = new List<ActionWordEnum>();
            if (playerData.currentChapter <= Chapter.chapter_1)
            {
                phonemes.AddRange(set1);
                currentSectionPercent = 1f;
            }
            else if (playerData.currentChapter == Chapter.chapter_2)
            {
                prevPhonemes.AddRange(set1);
                phonemes.AddRange(set2);
                currentSectionPercent = 0.7f;
            }
            else if (playerData.currentChapter == Chapter.chapter_3)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                phonemes.AddRange(set3);
                currentSectionPercent = 0.6f;
            }
            else if (playerData.currentChapter == Chapter.chapter_4)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                prevPhonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 0.5f;
            }
            else
            {
                phonemes.AddRange(set1);
                phonemes.AddRange(set2);
                phonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 1f;
            }
        }

        // Decide whether to use the current section or previous section for this selection
        float randNum = Random.Range(0f, 1f);
        if (randNum <= currentSectionPercent)
        {
            return phonemes[Random.Range(0, phonemes.Count)];
        }
        else
        {
            return prevPhonemes[Random.Range(0, phonemes.Count)];
        }
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawPol(ActionWordEnum coin, List<ChallengeWord> excludeWords = null, int difficultyLevel = -1, bool usingPracticeMode = false)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();
        List<ActionWordEnum> phonemes = new List<ActionWordEnum>();
        phonemes.Add(coin);

        if (difficultyLevel == -1)
        {
            difficultyLevel = 1 + Mathf.FloorToInt(playerData.starsBlend / 3);
        }

        // Safety Checks, bound difficulty level between 1-6 only
        if (difficultyLevel < 1)
        {
            difficultyLevel = 1;
        }
        if (difficultyLevel > 6)
        {
            difficultyLevel = 6;
        }

        int elkoninValueMin;
        int elkoninValueMax;
        bool similarSounds;
        ChallengeWord correctWord;

        if (difficultyLevel % 2 == 1)
        { // Odd difficulty levels have no similar sounds
            similarSounds = false;
        }
        else
        { // Even difficulty levels have similar sounds
            similarSounds = true;
        }

        // Assign Elkonin Value based on the difficulty
        if (difficultyLevel <= 2)
        {
            elkoninValueMin = 2;
            elkoninValueMax = 3;
        }
        else if (difficultyLevel == 3)
        {
            elkoninValueMin = 3;
            elkoninValueMax = 3;
        }
        else if (difficultyLevel == 4)
        {
            elkoninValueMin = 3;
            elkoninValueMax = 4;
        }
        else
        {
            elkoninValueMin = 4;
            elkoninValueMax = 10;
        }


        List<ActionWordEnum> set1 = new List<ActionWordEnum>()
        { ActionWordEnum.mudslide, ActionWordEnum.listen, ActionWordEnum.poop,
          ActionWordEnum.orcs, ActionWordEnum.think, ActionWordEnum.explorer };

        List<ActionWordEnum> set2 = new List<ActionWordEnum>()
        { ActionWordEnum.hello, ActionWordEnum.spider, ActionWordEnum.scared, ActionWordEnum.thatguy };

        List<ActionWordEnum> set3 = new List<ActionWordEnum>()
        { ActionWordEnum.choice, ActionWordEnum.strongwind, ActionWordEnum.pirate,
          ActionWordEnum.gorilla, ActionWordEnum.sounds, ActionWordEnum.give };

        List<ActionWordEnum> set4 = new List<ActionWordEnum>()
        { ActionWordEnum.backpack, ActionWordEnum.frustrating, ActionWordEnum.bumphead, ActionWordEnum.baby }; //, ActionWordEnum.hit };

        List<ActionWordEnum> incorrectPhonemes = new List<ActionWordEnum>();

        if (usingPracticeMode)
        {
            incorrectPhonemes.AddRange(set1);
            incorrectPhonemes.AddRange(set2);
            incorrectPhonemes.AddRange(set3);
            incorrectPhonemes.AddRange(set4);
        }
        else if (playerData.currentChapter <= Chapter.chapter_1)
        {
            incorrectPhonemes.AddRange(set1);
        }
        else if (playerData.currentChapter == Chapter.chapter_2)
        {
            incorrectPhonemes.AddRange(set1);
            incorrectPhonemes.AddRange(set2);
        }
        else if (playerData.currentChapter == Chapter.chapter_3)
        {
            incorrectPhonemes.AddRange(set1);
            incorrectPhonemes.AddRange(set2);
            incorrectPhonemes.AddRange(set3);
        }
        else if (playerData.currentChapter >= Chapter.chapter_4)
        {
            incorrectPhonemes.AddRange(set1);
            incorrectPhonemes.AddRange(set2);
            incorrectPhonemes.AddRange(set3);
            incorrectPhonemes.AddRange(set4);
        }

        if (incorrectPhonemes.Contains(coin))
        {
            incorrectPhonemes.Remove(coin);
        }

        // Get all challenge words for the main group of phonemes
        List<ChallengeWord> currSectionChallengeWords = new List<ChallengeWord>();
        currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));

        List<ChallengeWord> incorrectChallengeWords = new List<ChallengeWord>();
        incorrectChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(incorrectPhonemes));

        // Remove any words already used in this game
        foreach (ChallengeWord word in excludeWords)
        {
            if (currSectionChallengeWords.Contains(word))
            {
                currSectionChallengeWords.Remove(word);
            }
            if (incorrectChallengeWords.Contains(word))
            {
                incorrectChallengeWords.Remove(word);
            }
        }
        // If there aren't enough words to pick from, repopulate the list
        if (currSectionChallengeWords.Count < 1)
        {
            currSectionChallengeWords.Clear();
            currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        }
        // If there aren't enough words to pick from, repopulate the list
        if (incorrectChallengeWords.Count < 1)
        {
            incorrectChallengeWords.Clear();
            incorrectChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(incorrectPhonemes));
        }

        // Filtering out words with the incorrect elkonin values
        List<ChallengeWord> filteredCurrChallengeWords = new List<ChallengeWord>();
        List<ChallengeWord> filteredIncorrectChallengeWords = new List<ChallengeWord>();

        foreach (ChallengeWord word in currSectionChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredCurrChallengeWords.Add(word);
            }
        }
        foreach (ChallengeWord word in incorrectChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredIncorrectChallengeWords.Add(word);
            }
        }

        if (filteredCurrChallengeWords.Count < 1)
        {
            filteredCurrChallengeWords.Clear();
            filteredCurrChallengeWords.AddRange(currSectionChallengeWords);
        }
        if (filteredIncorrectChallengeWords.Count < 1)
        {
            filteredIncorrectChallengeWords.Clear();
            filteredIncorrectChallengeWords.AddRange(incorrectChallengeWords);
        }

        List<ChallengeWord> wordsToReturn = new List<ChallengeWord>();
        correctWord = filteredCurrChallengeWords[Random.Range(0, filteredCurrChallengeWords.Count)];
        wordsToReturn.Add(correctWord);

        // Remove the chose word from both lists
        filteredCurrChallengeWords.Remove(correctWord);
        if (filteredIncorrectChallengeWords.Contains(correctWord))
        {
            filteredIncorrectChallengeWords.Remove(correctWord);
        }

        if (filteredCurrChallengeWords.Count < 1)
        {
            Debug.LogWarning("Only 1 valid challenge word of elkonin count, refilling with all challenge words");
            filteredCurrChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        }

        if (difficultyLevel == 1)
        {
            int randIndex = Random.Range(0, filteredCurrChallengeWords.Count);
            wordsToReturn.Add(filteredCurrChallengeWords[randIndex]);
            if (filteredIncorrectChallengeWords.Contains(filteredCurrChallengeWords[randIndex]))
            {
                filteredIncorrectChallengeWords.Remove(filteredCurrChallengeWords[randIndex]);
            }
            filteredCurrChallengeWords.Remove(filteredCurrChallengeWords[randIndex]);
        }

        List<ChallengeWord> finalCurrChallengeWords = new List<ChallengeWord>();
        finalCurrChallengeWords.AddRange(filteredCurrChallengeWords);

        List<ChallengeWord> finalIncorrectChallengeWords = new List<ChallengeWord>();
        finalIncorrectChallengeWords.AddRange(filteredIncorrectChallengeWords);

        // If we are supposed to, filter for only words with the same beginning or ending sound
        if (similarSounds)
        {
            foreach (ChallengeWord word in filteredIncorrectChallengeWords)
            {
                if (word.elkoninList[0] != correctWord.elkoninList[0] && word.elkoninList[word.elkoninList.Count - 1] != correctWord.elkoninList[correctWord.elkoninCount - 1])
                {
                    finalIncorrectChallengeWords.Remove(word);
                }
            }

            if (finalIncorrectChallengeWords.Count < 4)
            {
                finalIncorrectChallengeWords.Clear();
                finalIncorrectChallengeWords.AddRange(filteredIncorrectChallengeWords);
            }
        }

        // Repeat selection process for the two incorrect words
        while (wordsToReturn.Count < 5)
        {
            int randIndex = Random.Range(0, finalIncorrectChallengeWords.Count);
            wordsToReturn.Add(finalIncorrectChallengeWords[randIndex]);
            finalIncorrectChallengeWords.Remove(finalIncorrectChallengeWords[randIndex]);

            if (finalIncorrectChallengeWords.Count < 1)
            {
                finalIncorrectChallengeWords.AddRange(filteredCurrChallengeWords);
            }
        }

        return wordsToReturn;
    }

    public static List<ChallengeWord> ChallengeWordSelectionTigerPawCoin(List<ChallengeWord> excludeWords = null, int difficultyLevel = -1, List<ActionWordEnum> phonemes = null)
    {
        StudentPlayerData playerData = StudentInfoSystem.GetCurrentProfile();

        if (difficultyLevel == -1)
        {
            difficultyLevel = 1 + Mathf.FloorToInt(playerData.starsTPawCoin / 3);
        }

        // Safety Checks, bound difficulty level between 1-6 only
        if (difficultyLevel < 1)
        {
            difficultyLevel = 1;
        }
        if (difficultyLevel > 6)
        {
            difficultyLevel = 6;
        }

        List<ActionWordEnum> set1 = new List<ActionWordEnum>()
        { ActionWordEnum.mudslide, ActionWordEnum.listen, ActionWordEnum.poop,
          ActionWordEnum.orcs, ActionWordEnum.think, ActionWordEnum.explorer };

        List<ActionWordEnum> set2 = new List<ActionWordEnum>()
        { ActionWordEnum.hello, ActionWordEnum.spider, ActionWordEnum.scared, ActionWordEnum.thatguy };

        List<ActionWordEnum> set3 = new List<ActionWordEnum>()
        { ActionWordEnum.choice, ActionWordEnum.strongwind, ActionWordEnum.pirate,
          ActionWordEnum.gorilla, ActionWordEnum.sounds, ActionWordEnum.give };

        List<ActionWordEnum> set4 = new List<ActionWordEnum>()
        { ActionWordEnum.backpack, ActionWordEnum.frustrating, ActionWordEnum.bumphead, ActionWordEnum.baby }; //, ActionWordEnum.hit };

        List<ActionWordEnum> prevPhonemes = new List<ActionWordEnum>();

        int elkoninValueMin;
        int elkoninValueMax;
        float currentSectionPercent = 1f;
        ChallengeWord correctWord;

        // Assign Elkonin Value based on the difficulty
        if (difficultyLevel <= 2)
        {
            elkoninValueMin = 2;
            elkoninValueMax = 3;
        }
        else if (difficultyLevel == 3 || difficultyLevel == 4)
        {
            elkoninValueMin = 3;
            elkoninValueMax = 3;
        }
        else
        {
            elkoninValueMin = 4;
            elkoninValueMax = 10;
        }

        // If phonemes weren't specified in the function parameters, assign phoneme groups and previous phoneme groups based on chapter
        // Additionally, assign the probability that the current phonemes get used over the previous phoneme group 0f - 1f
        if (phonemes == null)
        {
            prevPhonemes = new List<ActionWordEnum>();
            if (playerData.currentChapter <= Chapter.chapter_1)
            {
                phonemes.AddRange(set1);
                currentSectionPercent = 1f;
            }
            else if (playerData.currentChapter == Chapter.chapter_2)
            {
                prevPhonemes.AddRange(set1);
                phonemes.AddRange(set2);
                currentSectionPercent = 0.7f;
            }
            else if (playerData.currentChapter == Chapter.chapter_3)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                phonemes.AddRange(set3);
                currentSectionPercent = 0.6f;
            }
            else if (playerData.currentChapter == Chapter.chapter_4)
            {
                prevPhonemes.AddRange(set1);
                prevPhonemes.AddRange(set2);
                prevPhonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 0.5f;
            }
            else
            {
                phonemes.AddRange(set1);
                phonemes.AddRange(set2);
                phonemes.AddRange(set3);
                phonemes.AddRange(set4);
                currentSectionPercent = 1f;
            }
        }

        // Get all challenge words for the main group of phonemes
        List<ChallengeWord> currSectionChallengeWords = new List<ChallengeWord>();
        currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        //Debug.Log("Getting Challenge Words for current group of phonemes: " + phonemes.Count);
        //Debug.Log("Challenge Words: " + currSectionChallengeWords.Count);

        // If we are in a section with a previous group of phonemes, get all challenge words for that group
        if (prevPhonemes.Count < 1)
        {
            prevPhonemes.AddRange(phonemes);
        }
        List<ChallengeWord> prevSectionChallengeWords = new List<ChallengeWord>();
        prevSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(prevPhonemes));
        //Debug.Log("Getting Challenge Words for previous group of phonemes: " + prevPhonemes.Count);
        //Debug.Log("Previous Challenge Words: " + prevSectionChallengeWords.Count);

        // Remove any words already used in this game
        foreach (ChallengeWord word in excludeWords)
        {
            if (currSectionChallengeWords.Contains(word))
            {
                currSectionChallengeWords.Remove(word);
            }

            if (prevSectionChallengeWords.Contains(word))
            {
                prevSectionChallengeWords.Remove(word);
            }
        }
        // If there aren't enough words to pick from, repopulate the list
        if (currSectionChallengeWords.Count < 1)
        {
            currSectionChallengeWords.Clear();
            currSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(phonemes));
        }
        if (prevSectionChallengeWords.Count < 1)
        {
            prevSectionChallengeWords.Clear();
            prevSectionChallengeWords.AddRange(ChallengeWordDatabase.GetChallengeWords(prevPhonemes));
        }

        // Filtering out words with the incorrect elkonin values
        List<ChallengeWord> filteredCurrChallengeWords = new List<ChallengeWord>();
        List<ChallengeWord> filteredPrevChallengeWords = new List<ChallengeWord>();

        foreach (ChallengeWord word in currSectionChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredCurrChallengeWords.Add(word);
            }
        }
        foreach (ChallengeWord word in prevSectionChallengeWords)
        {
            if (word.elkoninCount >= elkoninValueMin && word.elkoninCount <= elkoninValueMax)
            {
                filteredPrevChallengeWords.Add(word);
            }
        }

        if (filteredCurrChallengeWords.Count < 1)
        {
            filteredCurrChallengeWords.AddRange(currSectionChallengeWords);
        }
        if (filteredPrevChallengeWords.Count < 1)
        {
            filteredPrevChallengeWords.AddRange(prevSectionChallengeWords);
        }

        // Decide whether to use the current section or previous section for this selection
        float randNum = Random.Range(0f, 1f);
        if (randNum <= currentSectionPercent)
        {
            correctWord = filteredCurrChallengeWords[Random.Range(0, filteredCurrChallengeWords.Count)];
            filteredCurrChallengeWords.Remove(correctWord);
            if (filteredPrevChallengeWords.Contains(correctWord))
            {
                filteredPrevChallengeWords.Remove(correctWord);
            }
        }
        else
        {
            correctWord = filteredPrevChallengeWords[Random.Range(0, filteredPrevChallengeWords.Count)];
            filteredPrevChallengeWords.Remove(correctWord);
            if (filteredCurrChallengeWords.Contains(correctWord))
            {
                filteredCurrChallengeWords.Remove(correctWord);
            }
        }

        //Debug.Log("Current words after removing correct: " + filteredCurrChallengeWords.Count);
        //Debug.Log("Previous words after removing correct: " + filteredPrevChallengeWords.Count);

        if (filteredCurrChallengeWords.Count < 1)
        {
            filteredCurrChallengeWords.Clear();
            filteredCurrChallengeWords.AddRange(currSectionChallengeWords);
        }
        if (filteredPrevChallengeWords.Count < 1)
        {
            filteredPrevChallengeWords.Clear();
            filteredPrevChallengeWords.AddRange(currSectionChallengeWords);
        }

        List<ChallengeWord> wordsToReturn = new List<ChallengeWord>();
        wordsToReturn.Add(correctWord);

        return wordsToReturn;
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
        //set4.Add(ActionWordEnum.hit);

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
        //set5.Add(ActionWordEnum.hit);

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
        //set4.Add(ActionWordEnum.hit);

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
        //set5.Add(ActionWordEnum.hit);


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
        //set4.Add(ActionWordEnum.hit);

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
        //set5.Add(ActionWordEnum.hit);


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
        //set4.Add(ActionWordEnum.hit);

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
        //set5.Add(ActionWordEnum.hit);


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
