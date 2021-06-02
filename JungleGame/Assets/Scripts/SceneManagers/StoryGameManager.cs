using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGameManager : MonoBehaviour
{   
    [Header("Dev Mode Stuff")]
    public StoryGameData devData;

    [Header("Game Object Variables")]
    [SerializeField] private LogCoin coin;
    [SerializeField] private DancingManController dancingMan;

    [Header("Shake Function Variables")]
    public float shakeDuration;
    public float shakeSpeed;
    public float shakeAmount;

    [Header("Text Variables")]
    [SerializeField] private Transform textLayoutGroup;
    [SerializeField] private GameObject textWrapperObject;
    [SerializeField] private Transform actionWordStopPos;
    public float textHeight;
    // text colors used 
    public Color defaultTextColor;
    public Color actionTextColor;

    private List<Transform> actionWords;
    private int currWord;





    private StoryGameData storyGameData;

    void Awake()
    {
        GameManager.instance.SceneInit();

        // load in game data from game manager
        GameData data = GameManager.instance.GetData();
        // make sure it is usable
        if (data == null || data.gameType != GameType.StoryGame)
        {
            // use dev data if in dev mode
            if (GameManager.instance.devModeActivated)
                storyGameData = devData;
            else // send error
                GameManager.instance.SendError(this, "invalid game data");
        }   
    }

    void Start()
    {
        PregameSetup();
        StartCoroutine(PartOneRoutine());
    }

    private void PregameSetup()
    {
        // make action word list
        actionWords = new List<Transform>();
        currWord = 0;

        // add the text objects to the layout group
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            var textObj = Instantiate(textWrapperObject, textLayoutGroup);
            textObj.GetComponent<TextWrapper>().SetText(seg.text);
            textObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);

            var wordObj = Instantiate(textWrapperObject, textLayoutGroup);
            wordObj.GetComponent<TextWrapper>().SetText(seg.actionWord.ToString());
            wordObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
            actionWords.Add(wordObj.transform);

            // add small space inbetween segments
            // var spaceObj = Instantiate(textWrapperObject, textLayoutGroup);
            // spaceObj.GetComponent<TextWrapper>().SetText("  ");
        }
    }   

    private IEnumerator PartOneRoutine()
    {
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            coin.SetCoinType(seg.actionWord);
            AudioManager.instance.PlayTalk(seg.audio);

            // move text until action word is in place
            StartCoroutine(MoveTextToNextActionWord(seg.audioDuration));

            yield return new WaitForSeconds(seg.audioDuration);

            AudioClip actionWordAudio = GameManager.instance.GetActionWord(seg.actionWord).audio;
            AudioManager.instance.PlayTalk(actionWordAudio);
            actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
            currWord++;
            dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
            ShakeCoin();
            
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator MoveTextToNextActionWord(float duration)
    {
        float start = textLayoutGroup.position.x;
        float end = start - Mathf.Abs(actionWordStopPos.position.x - actionWords[currWord].transform.position.x);
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > duration)
            {
                break;
            }

            float tempX = Mathf.Lerp(start, end, timer / duration);
            textLayoutGroup.position = new Vector3(tempX, textHeight, 0f);
            yield return null;
        }
    }

    private void ShakeCoin()
    {
        StartCoroutine(ShakeCoinRoutine(shakeDuration));
    }

    private IEnumerator ShakeCoinRoutine(float duration)
    {
        float timer = 0f;
        Vector3 originalPos = coin.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                coin.transform.position = originalPos;
                break;
            }

            Vector3 pos = originalPos;
            pos.x = originalPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            coin.transform.position = pos;
            yield return null;
        }
    }
}
