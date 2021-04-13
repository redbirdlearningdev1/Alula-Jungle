using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaController : MonoBehaviour
{
    private int currPos = 0;
    public const int maxPos = 5;
    private Animator animator;

    private List<Transform> currPath;
    private float moveSpeed = 7f;

    [Header("Jump Paths")]
    public List<Transform> jump1;
    public List<Transform> jump2;
    public List<Transform> jump3;
    public List<Transform> jump4;
    public List<Transform> jump5;


    void Awake() 
    {    
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            // jump 1
            if (Input.GetKeyDown(KeyCode.Z))
            {
                JumpBack();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                JumpForward();
            }
        }
    }

    public void CelebrateAnimation(float time = 1.5f)
    {
        StartCoroutine(CelebrateAnimationRoutine(time));
    }

    private IEnumerator CelebrateAnimationRoutine(float time)
    {
        animator.Play("gorilla_celebrate");
        yield return new WaitForSeconds(time);
        animator.Play("gorilla_idle");
    }

    public void JumpForward()
    {
        if (currPos < maxPos)
        {
            switch (currPos)
            {
                default:
                case 0:
                    currPath = jump1;
                    break;
                case 1:
                    currPath = jump2;
                    break;
                case 2:
                    currPath = jump3;
                    break;
                case 3:
                    currPath = jump4;
                    break;
                case 4:
                    currPath = jump5;
                    break;
            }
            currPos++;
            StartCoroutine(FollowPath(false));
        }
    }

    public void JumpBack()
    {
        if (currPos > 0)
        {
            switch (currPos)
            {
                default:
                case 5:
                    currPath = jump5;
                    break;
                case 4:
                    currPath = jump4;
                    break;
                case 3:
                    currPath = jump3;
                    break;
                case 2:
                    currPath = jump2;
                    break;
                case 1:
                    currPath = jump1;
                    break;
            }
            currPos--;
            StartCoroutine(FollowPath(true));
        }
    }

    private IEnumerator FollowPath(bool reverse)
    {   
        // reverse list
        if (reverse)
        {
            List<Transform> temp = new List<Transform>();
            for (int i = currPath.Count - 1; i >= 0; i--)
            {
                temp.Add(currPath[i]);
            }
            currPath = temp;
        }

        animator.Play("gorilla_prejump");
        yield return new WaitForSeconds(0.2f);

        int pathIndex = 0;
        float timer = 0f;
        float maxTime = 0.7f;
        Vector3 currTarget = currPath[pathIndex].position;
        Vector3 currStart = transform.position;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                
                transform.position = Vector3.Lerp(currStart, currTarget, timer / maxTime);
            }
            else
            {
                if (pathIndex < currPath.Count - 1)
                {
                    pathIndex++;
                    timer = 0;
                    currTarget = currPath[pathIndex].position;
                    currStart = transform.position;
                }
                else
                {
                    animator.Play("gorilla_afterjump");
                    yield break;
                }
            }
            yield return null;
        }   
    }
}
