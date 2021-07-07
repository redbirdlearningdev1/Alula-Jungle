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

    void Update()
    {
        // test star path animations
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                StartCoroutine(GrowObject(window));
                StartCoroutine(GrowObject(star1));
                StartCoroutine(GrowObject(star2));
                StartCoroutine(GrowObject(star3));
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // StartCoroutine(FollowPath(path1, star1.transform));
            }
        }
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
        if (numStars > 3 || numStars < 1)
        {
            GameManager.instance.SendError(this, "invalid number of stars awarded");
            return;
        }
        StartCoroutine(AwardStarsRoutine(numStars));
    }

    private IEnumerator AwardStarsRoutine(int numStars)
    {
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

        // TODO: use video - https://www.youtube.com/watch?v=11ofnLOE8pw&t=588s
        // animate stars into toolbar area
        // for (int i = 0; i < numStars; i++)
        // {
        //     switch (i)
        //     {
        //         default:
        //         case 0:
        //             //FollowPath(path1, star1.transform);
        //             break;
        //         case 1:
        //             //FollowPath(path2, star2.transform);
        //             break;
        //         case 2:
        //             //FollowPath(path3, star3.transform);
        //             break;
        //     }

        //     // time bewteen stars
        //     yield return new WaitForSeconds(0.5f);
        // }

        yield return new WaitForSeconds(0.5f);

        // update SIS

        // activate button
        button.interactable = true;
    }

    private IEnumerator GrowObject(GameObject gameObject)
    {
        StartCoroutine(ScaleObjectRoutine(gameObject, longScaleTime, maxScale));
        yield return new WaitForSeconds(longScaleTime);
        StartCoroutine(ScaleObjectRoutine(gameObject, shortScaleTime, normalScale));
    }

    private IEnumerator ShrimkObject(GameObject gameObject)
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
