using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RummageCoin : MonoBehaviour
{
    public ActionWordEnum type;
    public bool interactable;

    public Transform clothPos;
    public Transform coinParent;
    public Vector3 pileMovement1;
    public Vector3 pileMovement2;
    public Vector3 Origin;
    public float moveSpeed = 5f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private Image image;
    private bool audioPlaying;

    // original vars
    private bool originalSet = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play(type.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();
    }

    public void GoToChest()
    {
        print ("now!~");
        GetComponent<LerpableObject>().LerpScale(Vector2.zero, 0.2f);
        GetComponent<LerpableObject>().LerpPosition(chest.instance.transform.position, 0.2f, false);
        GetComponent<LerpableObject>().LerpImageAlpha(image, 0f, 0.2f);
        chest.instance.UpgradeBag();
    }

    public void ReturnToCloth()
    {
        StartCoroutine(ReturnToClothRoutine(clothPos.position));
    }

    public void setOrigin()
    {
        Origin = transform.position;
    }

    private IEnumerator ReturnToClothRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                yield break;
            }

            yield return null;
        }
    }

    public void grow()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(2.1f, 2.1f), new Vector2(2f, 2f), 0.2f, 0.2f);
    }
    public void shrink()
    {
        GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
    }

    public void BounceIn1()
    {
        StartCoroutine(BounceOutRoutine(Origin));
    }
    public void BounceOut1()
    {
        StartCoroutine(BounceOutRoutine(pileMovement1));
    }
    public void BounceToCloth()
    {
        StartCoroutine(BounceOutRoutine(clothPos.position));
    }

    private IEnumerator BounceOutRoutine(Vector3 target)
    {
        transform.SetParent(coinParent);
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                yield break;
            }
            yield return null;
        }
    }


    public void SetCoinType(ActionWordEnum type)
    {
        this.type = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();

        animator.Play(type.ToString());
    }

    public void PlayPhonemeAudio()
    {
        if (!audioPlaying)
        {
            StartCoroutine(PlayPhonemeAudioRoutine());
        }
    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(type);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    public void ToggleVisibility(bool opt)
    {
        if (opt) GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
        else GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
    }
}
