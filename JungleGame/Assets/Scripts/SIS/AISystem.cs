using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISystem
{
    private static List<GameType> minigameOptions; 
    private static float royalRumbleOdds = 0.05f; // 5%

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
                CreateMinigameList();
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

            // add other cases here
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
