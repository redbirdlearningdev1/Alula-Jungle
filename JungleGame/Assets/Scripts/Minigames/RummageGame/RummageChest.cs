using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummageChest : MonoBehaviour
{
    private Vector3 scaleNormal = new Vector3(53.3f, 53.3f, 0f);
    private Vector3 scaleSmall = new Vector3(0f, 53.3f, 0f);
    // Start is called before the first frame update

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void stretchOut()
    {
        StartCoroutine(stretchRoutine(scaleNormal));
    }
    public void stretchIn()
    {
        StartCoroutine(stretchRoutine(scaleSmall));
    }

    private IEnumerator stretchRoutine(Vector3 target)
    {
        Vector3 currStart = transform.localScale;
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

}
