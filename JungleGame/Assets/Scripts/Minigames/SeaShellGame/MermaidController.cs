using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MermaidController : MonoBehaviour
{
    public static MermaidController instance;

    public Animator blueAnimator;
    public Animator pinkAnimator;
    public Animator playAnimator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ShowMermaids()
    {
        StartCoroutine(ShowMermaidsRoutine());
    }

    private IEnumerator ShowMermaidsRoutine()
    {
        blueAnimator.Play("BlueRise");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.25f);
        pinkAnimator.Play("PinkRise");
    }

    public void PlayShell(int shellNum)
    {
        StartCoroutine(PlayShellRoutine(shellNum));
    }

    private IEnumerator PlayShellRoutine(int shellNum)
    {
        int mermaid = Random.Range(0, 2);

        if (mermaid == 0)
        {
            // blue dive
            blueAnimator.Play("BlueDive");
            yield return new WaitForSeconds(1f);

            switch (shellNum)
            {   
                case 1:
                    playAnimator.Play("BluePlay1");
                    break;
                case 2:
                    playAnimator.Play("BluePlay2");
                    break;
                case 3:
                    playAnimator.Play("BluePlay3");
                    break;
            }

            // play conch 1
            yield return new WaitForSeconds(0.25f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Conch1, 0.5f);

            // splash sound
            yield return new WaitForSeconds(1.75f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterSplash, 0.5f);

            // pink rise
            yield return new WaitForSeconds(1.5f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.25f);
            blueAnimator.Play("BlueRise");
        }
        else 
        {
            // pink dive
            pinkAnimator.Play("PinkDive");
            yield return new WaitForSeconds(1f);

            switch (shellNum)
            {   
                case 1:
                    playAnimator.Play("PinkPlay1");
                    break;
                case 2:
                    playAnimator.Play("PinkPlay2");
                    break;
                case 3:
                    playAnimator.Play("PinkPlay3");
                    break;
            }

            // play conch 1
            yield return new WaitForSeconds(0.25f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Conch2, 0.5f);

            // splash sound
            yield return new WaitForSeconds(1.75f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterSplash, 0.5f);

            // pink rise
            yield return new WaitForSeconds(1.5f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.25f);
            pinkAnimator.Play("PinkRise");
        }
    }
}
