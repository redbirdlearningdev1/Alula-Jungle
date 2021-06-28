using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public ActionWordEnum type;
    public Transform BallParent;
    public Vector3 MoveInPosition1;
    public Vector3 shakesF;
    public Vector3 shakesB;
    public Vector3 shakesInPlaceF;
    public Vector3 shakesInPlaceB;
    public Vector3 MoveInPosition2;
    public Vector3 MoveInPosition3;
    public Vector3 MoveInPosition4;
    public Vector3 MoveOutPosition1;
    public Vector3 MoveOutPosition2;
    public Vector3 MoveOutPosition3;
    public Vector3 MoveOutPosition4;
    public int choice;
    public Vector3 Origin;
    public Vector3 Goback;
    //public Vector3 Rotate = 
    private Vector3 scaleNormal = new Vector3(0.0046875f, 0.0046875f, 0f);
    private Vector3 scaleSmall = new Vector3(0.0031875f, 0.0031875f, 0f);
    public float moveSpeed = 1f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private Image image;


    // original vars
    private bool originalSet = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        string currType = type.ToString() + "b";
        print(currType);
        animator.Play(currType);

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();

    }

    void Update()
    {

    }


    public void setOrigin()
    {
        //Origin = transform.position;
    }
    public int getChoice()
    {
        return choice;
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




    public void MoveBack()
    {
        StartCoroutine(MoveBackRoutine(Goback, .5f));
    }

    public void MoveIn()
    {
        StartCoroutine(MoveInRoutine(Goback, 1f));
    }
    public void MoveToOrigin()
    {
        StartCoroutine(MoveBackRoutine(Origin, .5f));
    }
    private IEnumerator RotateRoutine(Vector3 target, float time)
    {
        Debug.Log("Here");
        //Vector3 currStart = transform.rotation;
        float timer = 0f;
        float maxTime = time;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                //transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                transform.rotation = Quaternion.Euler(Vector3.forward * 360);
            }
            else
            {
                transform.position = target;
                transform.SetParent(BallParent);
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator MoveBackRoutine(Vector3 target, float time)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = time;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                //transform.rotation = Quaternion.EulerAngles(Vector3.forward * 2);

            }
            else
            {
                transform.position = target;
                transform.SetParent(BallParent);
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator MoveInRoutine(Vector3 target, float time)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = time;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);

                
                //transform.Rotate(0, 0,1f * timer / maxTime * 360);

                    transform.Rotate(Vector3.back * 1.74f * timer / maxTime);

            }
            else
            {
                transform.position = target;
                transform.SetParent(BallParent);
                yield break;
            }

            yield return null;
        }
    }
    public void setToBaseRotation()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
    }
    public void MoveOver()
    {
        StartCoroutine(MoveOverRoutine(MoveInPosition1, 1f));
    }
    public void ShakeF()
    {
        StartCoroutine(MoveOverRoutine(shakesF, 1f));
    }
    public void ShakeB()
    {
        StartCoroutine(MoveOverRoutine(shakesB, 1f));
    }
    public void ShakeInF()
    {
        StartCoroutine(MoveOverRoutine(shakesInPlaceF, 1f));
    }
    public void ShakeInB()
    {
        StartCoroutine(MoveOverRoutine(shakesInPlaceB, 1f));
    }
    public void moveOut1()
    {
        StartCoroutine(MoveOverRoutine2(MoveOutPosition1, 1f));
    }
    public void moveOut2()
    {
        StartCoroutine(MoveOverRoutine2(MoveOutPosition2, 1f));
    }
    public void moveOut3()
    {
        StartCoroutine(MoveOverRoutine2(MoveOutPosition3, 1f));
    }
    public void moveOut4()
    {
        StartCoroutine(MoveOverRoutine2(MoveOutPosition4, 1f));
    }
    public void movePos2()
    {
        StartCoroutine(MoveOverRoutine(MoveInPosition2, 1f));
    }
    public void movePos3()
    {
        StartCoroutine(MoveOverRoutine(MoveInPosition3, 1f));
    }
    public void movePos4()
    {
        StartCoroutine(MoveOverRoutine2(MoveInPosition4, 1f));
    }
    private IEnumerator MoveOverRoutine(Vector3 target, float time)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = time;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);


                //transform.Rotate(0, 0,1f * timer / maxTime * 360);

                    transform.Rotate(Vector3.back * 1.74f * timer / maxTime);

            }
            else
            {
                transform.position = target;
                transform.SetParent(BallParent);
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator MoveOverRoutine2(Vector3 target, float time)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = time;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 2;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);


                //transform.Rotate(0, 0,1f * timer / maxTime * 360);

                transform.Rotate(Vector3.back * 1.74f * timer / maxTime);

            }
            else
            {
                transform.position = target;
                transform.SetParent(BallParent);
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
        animator.Play(type.ToString()+"b");
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
