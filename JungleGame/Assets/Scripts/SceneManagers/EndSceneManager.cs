using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public LerpableObject BG;
    public LerpableObject blurredBG;
    public LerpableObject blackBG;
    public Animator caveBG;

    public Animator marcusAnimator;
    public Animator brutusAnimator;

    void Awake()
    {
        GameManager.instance.SceneInit();

        AudioManager.instance.StopMusic();

        blurredBG.SetImageAlpha(blurredBG.GetComponent<Image>(), 0f);

        StartCoroutine(StartEndTalkies());
    }

    private IEnumerator StartEndTalkies()
    {
        yield return new WaitForSeconds(2f);

        // show blurred BG
        blurredBG.LerpImageAlpha(blurredBG.GetComponent<Image>(), 1f, 1f);

        // play end talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("End_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play end talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("End_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play end talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("End_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // show black BG
        blackBG.LerpImageAlpha(blackBG.GetComponent<Image>(), 1f, 1f);

        yield return new WaitForSeconds(3f);
        caveBG.Play("ShowCave");
        yield return new WaitForSeconds(4f);

        // play epilogue
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("Epilogue_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        marcusAnimator.Play("marcusWin");
        brutusAnimator.Play("brutusWin");

        // advance to last story beat
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.EndBossBattle)
        {
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        
        // go to credits scene
        GameManager.instance.LoadScene("CreditsScene", true, 3f, false);
    }

}
