using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISystem
{
    private static List<GameType> minigameOptions; 
    private static List<float> gameRatio;

    private static float royalRumbleOdds = 0.05f; // 5%

    public static GameType DetermineMinigame(StudentPlayerData playerData)
    {
        Debug.Log("Last Game Played " + playerData.lastGamePlayed);
        Debug.Log("Game Before Last Game Played " +playerData.gameBeforeLastPlayed);
        Debug.Log("Stars Last Game Played " +playerData.starsLastGamePlayed);
        Debug.Log("Stars Before Last Game Played " +playerData.starsGameBeforeLastPlayed);
        Debug.Log("Total Number of Stars Frogger " + playerData.totalStarsFrogger);
        Debug.Log("Total Number of Stars Sea "  + playerData.totalStarsSeashell);
        Debug.Log("Total Number of Stars Spider " + playerData.totalStarsSpiderweb);
        Debug.Log("Total Number of Stars Turn " + playerData.totalStarsTurntables);
        Debug.Log("Total Number of Stars Pirate " + playerData.totalStarsPirate);
        Debug.Log("Total Number of Stars Rummage " + playerData.totalStarsRummage);
        Debug.Log("Number of Stars Frogger " + playerData.starsFrogger);
        Debug.Log("Number of Stars Sea " + playerData.starsSeashell);
        Debug.Log("Number of Stars Spider " + playerData.starsSpiderweb);
        Debug.Log("Number of Stars Turn " + playerData.starsTurntables);
        Debug.Log("Number of Stars Pirate " + playerData.starsPirate);
        Debug.Log("Number of Stars Rummage " + playerData.starsRummage);

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
                    if(frogSuccessRatio == gameRatio[i] && addedFrog == 0)
                    {
                        minigameOptions.Add(GameType.FroggerGame);
                        addedFrog = 1;
                    }
                    if(seaSuccessRatio == gameRatio[i] && addedSea == 0)
                    {
                        minigameOptions.Add(GameType.SeashellGame);
                        addedSea = 1;
                    }
                    if(spiderSuccessRatio == gameRatio[i] && addedSpider == 0)
                    {
                        minigameOptions.Add(GameType.SpiderwebGame);
                        addedSpider = 1;
                    }
                    if(turnSuccessRatio == gameRatio[i] && addedTurn == 0)
                    {
                        minigameOptions.Add(GameType.TurntablesGame);
                        addedTurn = 1;
                    }
                    if(pirateSuccessRatio == gameRatio[i] && addedPirate == 0)
                    {
                        minigameOptions.Add(GameType.PirateGame);
                        addedPirate = 1;
                    }
                    if(rummageSuccessRatio == gameRatio[i] && addedRummage == 0)
                    {
                        minigameOptions.Add(GameType.RummageGame);
                        addedRummage = 1;
                    }
                }
                
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
                //else if(playerData.starsLastGamePlayed+playerData.starsGameBeforeLastPlayed >= 5)
                //{
                    
                //}
                else
                {
                    if(playerData.lastGamePlayed != minigameOptions[0])
                    {
                        return minigameOptions[0];
                    }
                    else if(playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed == 4)
                    {
                        return minigameOptions[2];
                    }
                    else if(playerData.starsLastGamePlayed + playerData.starsGameBeforeLastPlayed == 5)
                    {
                        return minigameOptions[3];
                    }
                    else
                    {
                        return minigameOptions[1];
                    }
                    
                }
                return minigameOptions[Random.Range(0, minigameOptions.Count)];
                
               
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
        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.WordFactoryBuilding);
        challengeGameOptions.Add(GameType.WordFactoryDeleting);
        challengeGameOptions.Add(GameType.WordFactorySubstituting);
        challengeGameOptions.Add(GameType.TigerPawCoins);
        challengeGameOptions.Add(GameType.TigerPawPhotos);
        challengeGameOptions.Add(GameType.Password);


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

        

        // return random index
        int index = Random.Range(0, challengeGameOptions.Count);
        return challengeGameOptions[index];
    }

    public static bool DetermineRoyalRumble(StudentPlayerData playerData)
    {
        // player must have played all 6 minigames before RR
        if (playerData.minigamesPlayed < 6)
            return false;

        // return false if royal rumble already active
        if (playerData.royalRumbleActive)
            return false;

        // determine royal rumble
        float num = Random.Range(0f, 1f);
        return num <= royalRumbleOdds;
    }

    public static GameType DetermineRoyalRumbleGame()
    {
        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.WordFactoryBuilding);
        challengeGameOptions.Add(GameType.WordFactoryDeleting);
        challengeGameOptions.Add(GameType.WordFactorySubstituting);
        challengeGameOptions.Add(GameType.TigerPawCoins);
        challengeGameOptions.Add(GameType.TigerPawPhotos);
        challengeGameOptions.Add(GameType.Password);
        
        // return random index
        int index = Random.Range(0, challengeGameOptions.Count);
        return challengeGameOptions[index];
    }
}
