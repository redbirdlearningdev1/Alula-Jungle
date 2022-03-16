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
        fillBar.transform.position = percent0Transform.position;
        bossBattleBar.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.5f);

        float currentBossBattlePoints = (float)StudentInfoSystem.GetCurrentProfile().bossBattlePoints;
        float targetXPos = Mathf.Lerp(percent0Transform.position.x, percent100Transform.position.x, currentBossBattlePoints / 99f);
        fillBar.LerpXPos(targetXPos, 1f, false);

        yield return new WaitForSeconds(1f);

        tigerHead.transform.position = tigerSpawnPos.position;
        tigerHead.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    public void HideBar()
    {
        StartCoroutine(HideBarRoutine());
    }

    private IEnumerator HideBarRoutine()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
        bossBattleBar.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.5f);

        fillBar.transform.position = percent0Transform.position;
        tigerHead.transform.localScale = Vector3.zero;
    }
}
