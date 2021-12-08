using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLock : MonoBehaviour
{
    public static PasswordLock instance;

    public Animator lockAnimator;

    private int currLockSprite = 0;
    private bool isShown = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    public bool LoseGame()
    {
        return currLockSprite >= 3;
    }

    public void ShowLock()
    {
        if (!isShown)
        {
            isShown = true;
            GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        }
    }
    
    public void HideLock()
    {
        if (isShown)
        {
            isShown = false;
            GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        }
    }

    public void Unlock()
    {
        lockAnimator.Play("Unlock");
    }

    public void ResetLock()
    {
        currLockSprite = 0;
        lockAnimator.Play("Locked1");
    }

    public void UpgradeLock()
    {
        if (!isShown)
            return;

        currLockSprite++;
        if (currLockSprite > 3)
            currLockSprite = 3;

        StartCoroutine(UpgradeLockAnim());
    }

    private IEnumerator UpgradeLockAnim()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.1f);
        
        switch (currLockSprite)
        {
            case 0:
                lockAnimator.Play("Locked1");
                break;
            case 1:
                lockAnimator.Play("Locked2");
                break;
            case 2:
                lockAnimator.Play("Locked3");
                break;
            case 3:
                lockAnimator.Play("Locked4");
                break;
        }
    }
}
