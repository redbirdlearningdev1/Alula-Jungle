using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleBar : MonoBehaviour
{
    public static BossBattleBar instance;

    public LerpableObject bossBattleBar;
    public LerpableObject fillBar;
    public LerpableObject tigerHead;

    public Transform percent0Transform;
    public Transform percent100Transform;
    public Transform tigerSpawnPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        bossBattleBar.transform.localScale = Vector3.zero;
        fillBar.transform.position = percent0Transform.position;
        tigerHead.transform.localScale = Vector3.zero;
    }

    public void ShowBar()
    {
        StartCoroutine(ShowBarRoutine());
    }

    private IEnumerator ShowBarRoutine()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
        fillBar.transform.localPosition = percent0Transform.localPosition;
        bossBattleBar.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.5f);

        float currentBossBattlePoints = (float)StudentInfoSystem.GetCurrentProfile().bossBattlePoints;
        float targetXPos = Mathf.Lerp(percent0Transform.localPosition.x, percent100Transform.localPosition.x, currentBossBattlePoints / 99f);

        if (currentBossBattlePoints != 0)
        {
            fillBar.LerpXPosSmooth(targetXPos, 1f, true);
            yield return new WaitForSeconds(1.5f);
        }

        Vector2 targetTigerScale = Vector2.one;
        // set wiggle + scale to be more violent based on story beat
        switch (StudentInfoSystem.GetCurrentProfile().currStoryBeat)
        {
            case StoryBeat.BossBattle1:
                tigerHead.GetComponent<WiggleController>().multiplier = 5;
                targetTigerScale = Vector2.one;
                break;

            case StoryBeat.BossBattle2:
                tigerHead.GetComponent<WiggleController>().multiplier = 15;
                targetTigerScale = new Vector2(1.1f, 1.1f);
                break;

            case StoryBeat.BossBattle3:
                tigerHead.GetComponent<WiggleController>().multiplier = 30;
                targetTigerScale = new Vector2(1.2f, 1.2f);
                break;
        }
        
        if (currentBossBattlePoints < 99f)
        {
            //tigerHead.transform.position = tigerSpawnPos.position;
            tigerHead.SquishyScaleLerp(targetTigerScale * 1.2f, targetTigerScale, 0.1f, 0.1f);
            tigerHead.GetComponent<WiggleController>().StartWiggle();
        }
        

        // start bobbing
        GetComponent<BobController>().StartBob();
    }

    public void HideBar()
    {
        StartCoroutine(HideBarRoutine());
    }

    private IEnumerator HideBarRoutine()
    {
        // stop bobbing
        GetComponent<BobController>().StopBob();

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
        bossBattleBar.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.5f);

        fillBar.transform.position = percent0Transform.position;
        tigerHead.transform.localScale = Vector3.zero;
        tigerHead.GetComponent<WiggleController>().StopWiggle();
    }
}
