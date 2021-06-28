using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateRopeController : MonoBehaviour
{
    public ActionWordEnum type;
    public Vector3 ropePos;
    public Vector3 Origin;
    private Vector3 scaleNormal = new Vector3(.25f, .25f, 0f);
    private Vector3 scaleSmall = new Vector3(.15f, .15f, 0f);
    public float moveSpeed = 1f;
    [SerializeField] private List<Sprite> ropeSprites;
    private Image image;

    // original vars
    private bool originalSet = false;

    void Awake()
    {

        RectTransform rt = GetComponent<RectTransform>();

        image = GetComponent<Image>();
//        ToggleVisibility(false, false);
    }

    void Update()
    {

    }
    public void breakRope()
    {
        image.sprite = ropeSprites[1];
    }
    public void fixRope()
    {
        image.sprite = ropeSprites[0];
    }

    public void setOrigin()
    {
        //Origin = transform.position;
    }
    public void GoToOrigin()
    {
        StartCoroutine(GoToOriginRoutine(Origin));
    }

    private IEnumerator GoToOriginRoutine(Vector3 target)
    {
        Debug.Log("Here");
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
        StartCoroutine(growRoutine(scaleNormal));
    }
    public void shrink()
    {
        StartCoroutine(growRoutine(scaleSmall));
    }

    private IEnumerator growRoutine(Vector3 target)
    {
        Vector3 currStart = transform.localScale;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 2;
            if (timer < maxTime)
            {
                transform.localScale = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.localScale = target;

                yield break;
            }

            yield return null;
        }
    }

    public void moveIn()
    {
        ToggleVisibility(true, true);
        StartCoroutine(moveInRoutine(ropePos));
    }


    private IEnumerator moveInRoutine(Vector3 target)
    {
        Debug.Log("Here");
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




    public void ToggleVisibility(bool opt, bool smooth)
    {
        Debug.Log("Toggle");
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!image)
                image = GetComponent<Image>();
            Color temp = image.color;
            if (opt) { temp.a = 1f; }
            else { temp.a = 0; }
            image.color = temp;
        }
    }

    private IEnumerator ToggleVisibilityRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 1f; }
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            Color temp = image.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            image.color = temp;

            if (image.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }


}
