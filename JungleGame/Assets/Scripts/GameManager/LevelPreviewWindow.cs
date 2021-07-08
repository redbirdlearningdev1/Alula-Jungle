using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelPreviewWindow : MonoBehaviour
{
    public static LevelPreviewWindow instance;

    [SerializeField] private GameObject window;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image gameImage;

    public float longScaleTime;
    public float shortScaleTime;

    public float hiddenScale;
    public float maxScale;
    public float normalScale;
    public float starMoveSpeed;

    private GameData gameData;
    private MapIconIdentfier id;
    private bool windowUp;

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
        
    }

    public void ResetWindow()
    {
        // set scales to be hidden
        star1.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        star2.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        star3.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
        window.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
    }

    public void NewWindow(GameData newGameData, MapIconIdentfier id, int numStars)
    {
        // return if another window is up
        if (windowUp)
            return;

        print("new window");

        if (numStars > 3 || numStars < 0)
        {
            GameManager.instance.SendError(this, "invalid number of stars");
            return;
        }

        windowUp = true;
        gameData = newGameData;
        titleText.text = gameData.gameType.ToString();
        SetGameImage(gameData.gameType);

        StartCoroutine(NewWindowRoutine(numStars));
    }

    // TODO: this
    private void SetGameImage(GameType gameType)
    {
        switch (gameType)
        {
            default:
            case GameType.FroggerGame:
                break;
        }
    }

    public void OnYesButtonPressed()
    {
        windowUp = false;
        // go to game scene
        GameManager.instance.SetDataAndID(gameData, id);
        GameManager.instance.LoadScene(gameData.sceneName, true);
    }

    public void OnNoButtonPressed()
    {
        windowUp = false;
        // hide window 
        StartCoroutine(ShrinkObject(window));
    }

    private IEnumerator NewWindowRoutine(int numStars)
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
