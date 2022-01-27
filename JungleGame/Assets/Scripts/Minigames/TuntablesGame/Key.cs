using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyNum;
    public Animator animator;
    public Transform origin;

    private ActionWordEnum currentWord;

    public void SetKeyType(ActionWordEnum word)
    {
        currentWord = word;
    }

    public ActionWordEnum GetKeyType()
    {
        return currentWord;
    }

    public void PlayAudio()
    {
        // wiggle key
        KeyWiggleAnim();
        
        // play correct audio
        AudioManager.instance.PlayPhoneme(currentWord);
    }

    public void ReturnToRope()
    {
        // make key normal size
        GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        // return to origin
        GetComponent<LerpableObject>().LerpPosToTransform(origin, 0.2f, false);
        // set origin as parent
        transform.SetParent(origin);
    }

    public void MoveIntoRockLock()
    {
        // make key normal size
        GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
        // return to origin
        GetComponent<LerpableObject>().LerpPosToTransform(TurntablesGameManager.instance.rockLock.transform, 0.25f, false);
    }

    public void KeyDownAnim()
    {
        switch (keyNum)
        {
            case 1: animator.Play("k1_last_move"); break;
            case 2: animator.Play("k2_last_move"); break;
            case 3: animator.Play("k3_last_move"); break;
            case 4: animator.Play("k4_last_move"); break;
        }
    }

    public void KeyUpAnim()
    {
        switch (keyNum)
        {
            case 1: animator.Play("k1_first_move"); break;
            case 2: animator.Play("k2_first_move"); break;
            case 3: animator.Play("k3_first_move"); break;
            case 4: animator.Play("k4_first_move"); break;
        }
    }

    public void KeyWiggleAnim()
    {
        switch (keyNum)
        {
            case 1: animator.Play("k1_audio_play"); break;
            case 2: animator.Play("k2_audio_play"); break;
            case 3: animator.Play("k3_audio_play"); break;
            case 4: animator.Play("k4_audio_play"); break;
        }
    }
}
