using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarAwardController : MonoBehaviour
{
    public static StarAwardController instance;

    [SerializeField] private GameObject window;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private Button button;

    public float longScaleTime;
    public float shortScaleTime;

    public float hiddenScale;
    public float maxScale;
    public float normalScale;

    public float starMoveSpeed;

    public GameObject coinObject;
    public Transform coinTarget;
    public Transform coinParent;
    private int newCoins = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ResetWindow();    
    }

    public void ResetWindow()
    {
        // set scales to be hidden
        star1.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        star2.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        star3.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        window.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);

        // deactivate button
        button.interactable = false;
    }

    public void AwardStarsAndExit(int numStars)
    {
        if (numStars > 3 || numStars < 0)
        {
            GameManager.instance.SendError(this, "invalid number of stars awarded");
            return;
        }

        // close settings menu if open
        SettingsManager.instance.CloseAllSettingsWindows();

        // remove settings button
        SettingsManager.instance.ToggleMenuButtonActive(false);

        // end split music (default)
        AudioManager.instance.EndSplitSong();

        // update SIS
        UpdateSIS(numStars);
    }

    private void UpdateSIS(int numStars)
    {
        int coinsEarned = 0;

        print ("map id: " + GameManager.instance.mapID);

        // determine if challenge game
        if (GameManager.instance.playingChallengeGame)
        {
            // lose?
            if (numStars <= 0)
            {
                // first time losing
                if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame)
                {
                    print ("first time losing challenge game!");
                    StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = true;
                }
                else
                {
                    // every other time losing
                    if (!StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
                    {
                        print ("every other time losing challenge game!");
                        StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = true;
                    }
                }
            }
            // win?
            else
            {
                print ("you won the challenge game!");
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = false;
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = false;
                StudentInfoSystem.AdvanceStoryBeat();
            }
        }
        // minigame stuff
        else
        {
            // increase number of minigames played
            StudentInfoSystem.GetCurrentProfile().minigamesPlayed += 1;
            print ("you coompleted a minigame, minigames played: " + StudentInfoSystem.GetCurrentProfile().minigamesPlayed);
        }

        // only update stars if earned more stars than in memory
        switch (GameManager.instance.mapID)
        {
            default:
                GameManager.instance.SendLog(this, "No ID for game found - not awarding stars nor coins");
                // show window
                StartCoroutine(AwardStarsRoutine(0, 0));
                return;

            /* 
            ################################################
            #   GORILLA VILLAGE
            ################################################
            */

            case MapIconIdentfier.GV_house1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GV_house2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GV_statue:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GV_fire:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GV_challenge_1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = numStars;
                }
                break;

            case MapIconIdentfier.GV_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.GV_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   MUDSLIDE
            ################################################
            */
        
            case MapIconIdentfier.MS_logs:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.MS_pond:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.MS_ramp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.MS_tower:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.MS_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.MS_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.MS_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = numStars;
                }
                break;


            /* 
            ################################################
            #   ORC VILLAGE
            ################################################
            */
        
            case MapIconIdentfier.OV_houseL:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OV_houseS:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OV_statue:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.OV_fire:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OV_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.OV_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.OV_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   SPOOKY FOREST
            ################################################
            */
        
            case MapIconIdentfier.SF_lamp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.SF_shrine:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.SF_spider:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.SF_web:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.SF_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.SF_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.SF_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   ORC CAMP
            ################################################
            */
        
            case MapIconIdentfier.OC_axe:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OC_bigTent:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OC_smallTent:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.OC_fire:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.OC_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.OC_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.OC_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   GORILLA POOP
            ################################################
            */
        
            case MapIconIdentfier.GP_outhouse1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GP_outhouse2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GP_rocks1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.GP_rocks2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GP_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.GP_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.GP_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   WINDY CLIFF
            ################################################
            */
        
            case MapIconIdentfier.WC_ladder:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.WC_lighthouse:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.WC_octo:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.WC_rock:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.WC_sign:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.WC_statue:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.WC_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.WC_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.WC_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   PIRATE SHIP
            ################################################
            */
        
            case MapIconIdentfier.PS_boat:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.PS_bridge:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.PS_front:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_front.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.PS_parrot:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.PS_sail:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.PS_wheel:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.PS_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.PS_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.PS_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   RUINS
            ################################################
            */
        
            case MapIconIdentfier.R_arch:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_arch.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.R_caveRock:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.R_face:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_face.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.R_lizard1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.R_lizard2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.R_pyramid:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.R_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.R_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.R_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   EXIT JUNGLE
            ################################################
            */

            case MapIconIdentfier.EJ_bridge:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.EJ_puppy:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.EJ_sign:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.EJ_torch:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.EJ_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.EJ_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.EJ_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   GORILLA STUDY
            ################################################
            */

            case MapIconIdentfier.GS_fire:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.GS_statue:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GS_tent1:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GS_tent2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.GS_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.GS_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.GS_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars = numStars;
                }
                break;

            /* 
            ################################################
            #   MONKEYS
            ################################################
            */

            case MapIconIdentfier.M_bananas:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;
            
            case MapIconIdentfier.M_flower:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.M_flower.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.M_guards:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.M_guards.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.M_tree:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars = numStars;
                }
                if (!StudentInfoSystem.GetCurrentProfile().mapData.M_tree.isFixed)
                {
                    GameManager.instance.repairMapIconID = true;
                }
                break;

            case MapIconIdentfier.M_challenge_1:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars < numStars)
                    {
                        coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars, numStars);
                        StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars = numStars;
                    }
                    break;

            case MapIconIdentfier.M_challenge_2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars = numStars;
                }
                break;

            case MapIconIdentfier.M_challenge_3:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars < numStars)
                {
                    coinsEarned = CalculateAwardedCoins(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars, numStars);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars = numStars;
                }
                break;
        }

        // determine if royal rumble game
        if (GameManager.instance.playingRoyalRumbleGame)
        {
            GameManager.instance.playingRoyalRumbleGame = false;
            GameManager.instance.finishedRoyalRumbleGame = true;
            GameManager.instance.wonRoyalRumbleGame = numStars > 0;

            StudentInfoSystem.GetCurrentProfile().royalRumbleActive = false;
            StudentInfoSystem.GetCurrentProfile().royalRumbleGame = GameType.None;
            
            GameManager.instance.repairMapIconID = true;
            GameManager.instance.mapID = StudentInfoSystem.GetCurrentProfile().royalRumbleID;
            StudentInfoSystem.GetCurrentProfile().royalRumbleID = MapIconIdentfier.None;
        }
        
        // save data
        StudentInfoSystem.SaveStudentPlayerData();
        // show window
        StartCoroutine(AwardStarsRoutine(numStars, coinsEarned));
    }

    private int CalculateAwardedCoins(int prevCoins, int newScore)
    {
        int newCoins = newScore - prevCoins;
        return newCoins;
    }

    private IEnumerator AwardStarsRoutine(int numStars, int coinsEarnded)
    {
        // add default background and raycast blocker
        DefaultBackground.instance.Activate();

        // show window
        StartCoroutine(GrowObject(window));
        yield return new WaitForSeconds(0.5f);

        print ("num stars: " + numStars);

        // show appropriate number of stars
        for (int i = 0; i < numStars; i++)
        {
            switch (i)
            {
                default:
                case 0:
                    StartCoroutine(GrowObject(star1));
                    break;
                case 1:
                    StartCoroutine(GrowObject(star2));
                    break;
                case 2:
                    StartCoroutine(GrowObject(star3));
                    break;
            }

            // play audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink1, 0.5f, "star_sound", 1f + 0.2f * i);

            // time bewteen stars
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        // show toolbar
        DropdownToolbar.instance.ToggleToolbar(true);
        int coinsEarndedCopy = coinsEarnded;
        newCoins = 1;
        
        // animate stars that are awarding coins
        for (int i = numStars - 1; i >= 0; i--)
        {
            // stop awarding coins when 0 is reached
            if (coinsEarndedCopy == 0)
                break;

            switch (i)
            {
                default:
                case 2:
                    StartCoroutine(StarAwardCoinAnimation(star3));
                    break;
                case 1:
                    StartCoroutine(StarAwardCoinAnimation(star2));
                    break;
                case 0:
                    StartCoroutine(StarAwardCoinAnimation(star1));
                    break;
            }
            // reduce coins earned after one is given
            coinsEarndedCopy--;

            // play audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "star_sound", 1f + 0.2f * i);

            // time bewteen stars
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // update SIS and toolbar
        DropdownToolbar.instance.AwardGoldCoins(coinsEarnded);

        // activate button
        button.interactable = true;
    }

    private IEnumerator StarAwardCoinAnimation(GameObject star)
    {
        StartCoroutine(ScaleObjectRoutine(star, longScaleTime, maxScale));
        yield return new WaitForSeconds(longScaleTime);

        // place particle system thing here
        StartCoroutine(AwardCoinAnimation(star.transform.position));

        StartCoroutine(ScaleObjectRoutine(star, longScaleTime, normalScale));
    }

    private IEnumerator GrowObject(GameObject gameObject)
    {
        StartCoroutine(ScaleObjectRoutine(gameObject, longScaleTime, maxScale));
        yield return new WaitForSeconds(longScaleTime);
        StartCoroutine(ScaleObjectRoutine(gameObject, shortScaleTime, normalScale));
    }

    private IEnumerator ShrinkObject(GameObject gameObject)
    {
        StartCoroutine(ScaleObjectRoutine(gameObject, shortScaleTime, maxScale));
        yield return new WaitForSeconds(shortScaleTime);
        StartCoroutine(ScaleObjectRoutine(gameObject, longScaleTime, hiddenScale));
    }

    private IEnumerator ScaleObjectRoutine(GameObject gameObject, float time, float scale)
    {
        float start = gameObject.transform.localScale.x;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                gameObject.transform.localScale = new Vector3(scale, scale, 0f);
                break;
            }

            float temp = Mathf.Lerp(start, scale, timer / time);
            gameObject.transform.localScale = new Vector3(temp, temp, 0f);
            yield return null;
        }
    }

    private IEnumerator AwardCoinAnimation(Vector3 startPos)
    {
        GameObject coin = Instantiate(coinObject, startPos, Quaternion.identity, coinParent);
        coin.transform.localScale = new Vector3(0f, 0f, 1f);
        coin.GetComponent<LerpableObject>().LerpPosToTransform(coinTarget, 0.5f, false);
        coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.5f, 1.5f), new Vector2(0f, 0f), 0.25f, 0.25f);
        yield return new WaitForSeconds(0.5f);

        // play audio
        AudioManager.instance.PlayCoinDrop();
        coinTarget.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        DropdownToolbar.instance.SetGoldText((StudentInfoSystem.GetCurrentProfile().goldCoins + newCoins).ToString());
        newCoins++;
    }
}
