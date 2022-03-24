using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeWordTestSceneManager : MonoBehaviour
{
    public TMP_Dropdown challengeWordDropdown;
    public Polaroid polaroid;

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
        print ("ahoy!");

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
        polaroid.SetPolaroid(globalChallengeWordList[challengeWordDropdown.value]);
        polaroid.LerpScale(1f, 0.1f);
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
