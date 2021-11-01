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

    [Header("Star Paths")]
    public List<Transform> path1;
    public List<Transform> path2;
    public List<Transform> path3;

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
        SettingsManager.instance.CloseSettingsWindow();

        // end split music (default)
        AudioManager.instance.EndSplitSong();

        // update SIS
        UpdateSIS(numStars);
    }

    private void UpdateSIS(int numStars)
    {
        int coinsEarned = 0;

        print ("map id: " + GameManager.instance.mapID);

        // only update stars if earned more stars than in memory
        switch (GameManager.instance.mapID)
        {
            default:
                GameManager.instance.SendLog(this, "No ID for game found - not awarding stars");
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
        }

        // challenge game stuff
        if (GameManager.instance.playingChallengeGame)
        {
            // lose?
            if (numStars <= 0)
            {
                // first time losing
                if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame)
                    StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = true;
                else
                {
                    // every other time losing
                    if (!StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
                    {
                        StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = true;
                    }
                }
            }
            // win?
            else
            {
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = false;
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = false;
                StudentInfoSystem.AdvanceStoryBeat();
            }

            GameManager.instance.playingChallengeGame = false;
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

            // time bewteen stars
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // show toolbar
        DropdownToolbar.instance.ToggleToolbar(true);
        
        // animate stars that are awarding coins
        for (int i = numStars - 1; i >= 0; i--)
        {
            // stop awarding coins when 0 is reached
            if (coinsEarnded == 0)
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
            coinsEarnded--;

            // time bewteen stars
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // activate button
        button.interactable = true;
    }

    private IEnumerator StarAwardCoinAnimation(GameObject star)
    {
        StartCoroutine(ScaleObjectRoutine(star, longScaleTime, maxScale));
        yield return new WaitForSeconds(longScaleTime);
        // place particle system thing here

        // update SIS and toolbar
        DropdownToolbar.instance.AwardGoldCoins(1);

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
}
