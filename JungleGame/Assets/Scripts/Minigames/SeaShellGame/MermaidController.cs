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

    private IEnumerator PlayShellSoundReverbRoutine(AudioClip clip)
    {
        AudioManager.instance.PlayFX_oneShot(clip, 0.6f, "shell1", 0.9f);
        yield return new WaitForSeconds(0.005f);
        AudioManager.instance.PlayFX_oneShot(clip, 0.2f, "shell2", 0.9f);
        yield return new WaitForSeconds(0.005f);
        AudioManager.instance.PlayFX_oneShot(clip, 0.1f, "shell3", 0.9f);
        yield return new WaitForSeconds(0.005f);
        AudioManager.instance.PlayFX_oneShot(clip, 0.05f, "shell4", 0.9f);

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
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell1.value).audio));
                    break;
                case 2:
                    playAnimator.Play("BluePlay2");
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell2.value).audio));
                    break;
                case 3:
                    playAnimator.Play("BluePlay3");
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell3.value).audio));
                    break;
            }

            // splash sound
            yield return new WaitForSeconds(1.8f);
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
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell1.value).audio));
                    break;
                case 2:
                    playAnimator.Play("PinkPlay2");
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell2.value).audio));
                    break;
                case 3:
                    playAnimator.Play("PinkPlay3");
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(PlayShellSoundReverbRoutine(GameManager.instance.GetActionWord(ShellController.instance.shell3.value).audio));
                    break;
            }

            // splash sound
            yield return new WaitForSeconds(1.8f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterSplash, 0.5f);

            // pink rise
            yield return new WaitForSeconds(1.5f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.25f);
            pinkAnimator.Play("PinkRise");
        }
    }
}
