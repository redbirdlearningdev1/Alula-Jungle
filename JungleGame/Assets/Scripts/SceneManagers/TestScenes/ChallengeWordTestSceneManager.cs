using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChallengeWordTestSceneManager : MonoBehaviour
{
    public TMP_Dropdown challengeWordDropdown;
    public Polaroid polaroid;
    public UniversalCoinImage setCoin;
    public GameObject universalCoin;
    public Transform coinParent;

    private ChallengeWord currentWord;
    private bool polaroidAnimating;

    public static List<ChallengeWord> globalChallengeWordList;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // remove music
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // show settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // get global challenge word list
        ChallengeWordDatabase.InitCreateGlobalList(); 
        globalChallengeWordList = ChallengeWordDatabase.globalChallengeWordList;

        // add talkie objects to dropdown
        List<string> challengeWordStringList = new List<string>();
        for(int i = 0; i < globalChallengeWordList.Count; i++)
        {
            challengeWordStringList.Add("" + i + " - " + globalChallengeWordList[i].word);
        }
        challengeWordDropdown.ClearOptions();
        challengeWordDropdown.AddOptions(challengeWordStringList);
        challengeWordDropdown.value = 0;
        // select first challenge word
        OnDropdownSelect();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        PlayCoin(result.gameObject.GetComponent<UniversalCoinImage>());
                    }
                    else if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        PlayPolaroid();
                    }
                }
            }
        }
    }

    private void PlayCoin(UniversalCoinImage coin)
    {
        StartCoroutine(PlayCoinRoutine(coin));
    }

    private IEnumerator PlayCoinRoutine(UniversalCoinImage coin)
    {
        coin.LerpScale(new Vector2(2f, 2f), 0.1f);
        coin.PlayAudio();
        yield return new WaitForSeconds(0.1f);
        coin.LerpScale(new Vector2(2.5f, 2.5f), 0.1f);
    }

    private void PlayPolaroid()
    {
        StartCoroutine(PlayPolaroidRoutine());
    }

    private IEnumerator PlayPolaroidRoutine()
    {
        polaroid.LerpScale(0.9f, 0.1f);
        AudioManager.instance.PlayTalk(currentWord.audio);
        yield return new WaitForSeconds(0.1f);
        polaroid.LerpScale(1f, 0.1f);
    }

    public void OnDropdownSelect()
    {
        // return if polaroid is animating
        if (polaroidAnimating)
            return;
        polaroidAnimating = true;

        StartCoroutine(SwitchPolaroidRoutine());
    }

    private IEnumerator SwitchPolaroidRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        // animate polaroid changing words
        polaroid.LerpScale(1.2f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        currentWord = globalChallengeWordList[challengeWordDropdown.value];
        print ("current word: " + currentWord.word);
        polaroid.SetPolaroid(currentWord);
        polaroid.LerpScale(1f, 0.1f);

        // remove all old coins + set coin
        setCoin.LerpScale(Vector2.zero, 0.1f);
        foreach (Transform coin in coinParent)
        {
            Destroy(coin.gameObject);
        }

        // set invisible frames
        InvisibleFrameLayout.instance.SetNumberOfFrames(currentWord.elkoninCount);
        yield return new WaitForSeconds(0.1f);
        // add new coins
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            GameObject coin = Instantiate(universalCoin, InvisibleFrameLayout.instance.frames[i].transform.position, Quaternion.identity, coinParent);
            coin.GetComponent<UniversalCoinImage>().SetValue(currentWord.elkoninList[i]);
            coin.GetComponent<UniversalCoinImage>().LerpScale(new Vector2(2.5f, 2.5f), 0.1f);
        }
        // set set coin
        setCoin.SetActionWordValue(currentWord.set);
        setCoin.LerpScale(new Vector2(2f, 2f), 0.1f);
        // say polaroid
        AudioManager.instance.PlayTalk(currentWord.audio);
        yield return new WaitForSeconds(0.1f);
        polaroidAnimating = false;
    }

    public void OnLeftButtonPressed()
    {
        // return if polaroid is animating
        if (polaroidAnimating)
            return;
        polaroidAnimating = true;

        // reduce value by one
        int newvalue = challengeWordDropdown.value - 1;
        // set to max value if value under 0
        if (newvalue < 0)
            newvalue = globalChallengeWordList.Count - 1;
        challengeWordDropdown.value = newvalue;
        StartCoroutine(SwitchPolaroidRoutine());
    }

    public void OnRightButtonPressed()
    {
        // return if polaroid is animating
        if (polaroidAnimating)
            return;
        polaroidAnimating = true;
        
        // reduce value by one
        int newvalue = challengeWordDropdown.value + 1;
        // set to max value if value under 0
        if (newvalue >= globalChallengeWordList.Count)
            newvalue = 0;
        challengeWordDropdown.value = newvalue;
        StartCoroutine(SwitchPolaroidRoutine());
    }
}
